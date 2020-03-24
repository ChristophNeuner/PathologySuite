window.ViewHisto = (divId, imgPath, xmlns, format, overlap, tileSize, height, width) => {
    var viewer = OpenSeadragon({
        id: divId,
        tileSources: {
            Image: {
                xmlns: xmlns,
                Url: imgPath,
                Format: format,
                Overlap: overlap,
                TileSize: tileSize,
                Size: {
                    Height: height,
                    Width: width
                }
            }
            
            //for non pyramid like fileformats like png
            //tileSources: {
            //    type: 'image',
            //    url: imgPath
            //}
        }
    });
};
