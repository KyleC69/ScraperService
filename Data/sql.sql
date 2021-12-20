INSERT INTO TumBlogs
    (
    BlogId,
    BlogName,
    BlogUrl,
    LastScanDate,
    LastScanStatus,
    LastScannedPostId,
    IsEnabled
    )
VALUES
    (
        5555,
        'My Feed',
        'https://www.newtumbl.com/feed',
        'LastScanDate:datetime2',
        'LastScanStatus:nvarchar',
        0,
        '1'
  );/*
Select tb.BlogName, vl.VidUrl, vl.IsDownloaded
from VidLinks vl inner join
    Tumblogs tb on tb.BlogId = vl.BlogId
where vl.isdownloaded =0

*/

use ScraperBlogs
select *
from NewTumblBlogs
where BlogEnabled=1



set BlogUrl='https://newtumbl.com/feed' where BlogID = 70655
order by BlogId
where IsDownloaded=0

--Selective Delete statement

/*
delete
from TumBlogs
where blogid
in
(
select  BlogId
from VidLinks as v
group by BlogId
having count(VidLinkId) < 10)
*/



delete
from TumBlogs
where blogid
not in
(
select BlogId
from VidLinks as v
group by BlogId)