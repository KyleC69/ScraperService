SET IDENTITY_INSERT TumBlogs ON
GO
-- Create the stored procedure in the specified schema
CREATE PROCEDURE [dbo].[InsertNewTumBlog]

    @BlogId /*parameter name*/ int,
    /*datatype_for_param1*/
    /*default_value_for_param1*/

    @BlogName /*parameter name*/ nvarchar(50),
    /*datatype_for_param1*/
    /*default_value_for_param2*/

    @BlogUrl  nvarchar(200)


-- add more stored procedure parameters here
AS
-- body of the stored procedure
BEGIN
    SET IDENTITY_INSERT TumBlogs ON
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
