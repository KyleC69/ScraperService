--Set IDENTITY_INSERT TumBlogs ON


/*
INSERT INTO VidLinks (
    
    VidUrl,
    AddedDate,
    PostId,
    IsDownloaded,
    BlogId
  )
select VidUrl,
    AddedDate,
    PostId,
    IsDownloaded,
    BlogId from VidAsset
*/

    select * from VidLinks innerjoin  Ass where IsDownloaded=0

delete from TumBlogs where BlogUrl is null
select * from Tumblogs    select * from Tumblogs