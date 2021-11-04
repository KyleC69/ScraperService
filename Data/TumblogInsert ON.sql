SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





--Set IDENTITY_INSERT TumBlogs ON
--go


-- Create the stored procedure in the specified schema
CREATE PROCEDURE [dbo].[TumBlogs_Update]

    @BlogId /*parameter name*/ int,
    /*datatype_for_param1*/
    /*default_value_for_param1*/

    @BlogName /*parameter name*/ nvarchar(50),
    /*datatype_for_param1*/
    /*default_value_for_param2*/
    @BlogUrl  nvarchar(200)
AS
BEGIN
UPDATE TumBlogs
Set BlogName = @BlogName,
BlogUrl = @BlogUrl
WHERE BlogId = @BlogId;
END
GO


Create PROCEDURE [dbo].[TumBlogs_Delete]
@BlogId int
AS
BEGIN
Update TumBlogs
set IsEnabled=1
where BlogID = @BlogID;
END
GO
/*
AS
BEGIN
  INSERT into TumBlogs( BlogName, BlogID, BlogUrl) 
  VALUES (@BlogName, @BlogID, @BlogUrl);
END
GO
*/

/*
-- body of the stored procedure
BEGIN

    MERGE INTO TumBlogs WITH (HOLDLOCK) t
     using
     (VALUES
        (@BlogName, @blogid, @BlogUrl))
    as [source] (BlogName, BlogID, BlogUrl)
ON
     t.BlogUrl = source.BlogUrl
    WHEN NOT MATCHED THEN
    INSERT (BlogName, BlogID, BlogUrl) VALUES (source.BlogName, source.BlogID, source.BlogUrl);


END

GO
*/