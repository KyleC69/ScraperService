/*

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Create the stored procedure in the specified schema
ALTER PROCEDURE [dbo].[GetAssetsToDownload]
    @qty /*parameter name*/ int /*datatype_for_param1*/ = 25 /*default_value_for_param1*/
-- add more stored procedure parameters here
---AS
    -- body of the stored procedure
--    Select top(25) vl.VidLinkId, vl.BlogID, vl.VidUrl, tb.BlogName from 
  --  VidLinks vl inner join TumBlogs tb on tb.BlogID = vl.BlogID where vl.IsDownloaded=0 


--GO




-- Select top(25) vl.VidLinkId, vl.BlogID, vl.VidUrl, tb.BlogName from 
--    VidLinks vl inner join TumBlogs tb on tb.BlogID = vl.BlogID where vl.IsDownloaded=0 


*/


select *
from VidLinks
where IsDownloaded=0



