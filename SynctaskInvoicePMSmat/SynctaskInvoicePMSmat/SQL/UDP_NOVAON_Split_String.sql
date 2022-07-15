USE [SALE_THUE]
GO
/****** Object:  UserDefinedFunction [dbo].[UDP_NOVAON_Split_String]    Script Date: 09/24/2020 08:47:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[UDP_NOVAON_Split_String] 
( 
--	@string VARCHAR(MAX) SQL 2005 above, 
    @string VARCHAR(8000),
    @delimiter CHAR(1) 
) 
RETURNS @output TABLE(splitdata NVARCHAR(50))
AS
BEGIN 
    DECLARE @start INT, @end INT 
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
    WHILE @start < LEN(@string) + 1 BEGIN 
        IF @end = 0  
            SET @end = LEN(@string) + 1
       
        INSERT INTO @output (splitdata)  
        VALUES(SUBSTRING(@string, @start, @end - @start)) 
        SET @start = @end + 1 
        SET @end = CHARINDEX(@delimiter, @string, @start)       
    END;
    RETURN
END