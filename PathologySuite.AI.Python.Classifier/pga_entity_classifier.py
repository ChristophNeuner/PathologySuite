### pituitary gland adenoma classifier
# see https://github.com/ChristophNeuner/pituitary_gland_adenomas/blob/master/inference_pipelines.ipynb

def predict_pga_entity(path_exported_learner:Union[pathlib.Path, str], wsi_path:Union[pathlib.Path, str])->Dict:
    """
    Arguments:
        path_exported_learner: path to file that has been exported via fastai.basic_train.Learner.export()
        wsi_path: path to a whole-slide image
    Returns:
        returns a dictionary with probabilities for the classes: ['acth', 'silent', 'lh', 'fsh']
    """
    # calculate tiles 
    tilesummaries = tiles.WsiOrROIToTilesMultithreaded(wsiPaths=[wsi_path], 
                                                    tilesFolderPath=None, 
                                                    tileHeight=512, 
                                                    tileWidth=512, 
                                                    tile_naming_func=tiles.get_wsi_name_from_path_pituitary_adenoma_entities,
                                                    tile_score_thresh=0.7,
                                                    tileScoringFunction=tiles.scoring_function_1,
                                                    is_wsi=True,
                                                    level=0,
                                                    save_tiles=False,
                                                    return_as_tilesummary_object=True)
    ts = tilesummaries[0]
    
    
    
    
    #overwrite fastai's function for opening images !!!TODO: change this by using a custom DataLoader Implementation
    def open_custom(self, fn):
        "Open image in `fn`."
        return open_image_custom(fn, convert_mode=self.convert_mode, after_open=self.after_open)

    def open_image_custom(fn:typing.Union[pathlib.Path, str], 
                      div:bool=True, 
                      convert_mode:str='RGB', 
                      cls:type=fastai.vision.Image, 
                      after_open:Callable=None)->fastai.vision.Image:
        "Open image in `fn`."
        fn = Path(fn)
        tile_name = fn.name
        t = tile_name_to_tile_object[tile_name]
        tile = tiles.ExtractTileFromWSI(path=t.wsi_path, 
                                        x=t.get_x(), 
                                        y=t.get_y(), 
                                        width=t.get_width(), 
                                        height=t.get_height(), 
                                        level=t.level)
        tile = tile.convert(convert_mode)
        if after_open: 
            tile = after_open(tile)
        tile = pil2tensor(tile,np.float32)
        if div: 
            tile.div_(255)
        return cls(tile)
        
    fastai.vision.data.ImageList.open = open_custom
    fastai.vision.image.open_image = open_image_custom
    
    #create fastai.vision.data.ImageList
    tiles_df = pd.DataFrame([t.get_name() for t in ts.top_tiles()], columns=['name'])

    tile_name_to_tile_object = {}
    for t in ts.top_tiles():
        tile_name_to_tile_object[t.get_name()] = t
    
    img_list = ImageList.from_df(df=tiles_df, path='')
    
    #init learner
    learner = load_learner(path=Path(path_exported_learner).parent, file=Path(path_exported_learner).name, test=img_list)
    learner.data.batch_size = 6
    
    #make predictions on tiles
    preds, y = learner.get_preds(ds_type=fastai.basic_data.DatasetType.Test)
    
    #calculate probabilities for the whole-slide image
    thresh = 0.5
    preds_bool = (preds > thresh).float()
    probs_wsi = preds_bool.sum(0)/len(preds_bool)
    classes = ['acth', 'silent', 'lh', 'fsh']
    result = {}
    for n, c in enumerate(classes):
        result[c] = probs_wsi[n].item()
    return result

