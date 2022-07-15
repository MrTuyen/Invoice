USE [SALE_THUE]
GO
/****** Object:  StoredProcedure [dbo].[USP_NOVAON_GET_INVOICES]    Script Date: 09/24/2020 08:48:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		TuyenNV
-- Create date: 12/08/2020
-- Description:	Lấy những bản ghi chưa xuất hóa đơn và có NgayHoaDon >= 5 phút trước thời điểm quét
-- =============================================
create PROCEDURE [dbo].[USP_NOVAON_GET_INVOICES]
	@RequestedTime DATETIME
AS
BEGIN	
	SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
	SET NOCOUNT ON;
	
	DECLARE @MinuteNumber INT;
	DECLARE @PreviousTime DATETIME;

	SET @MinuteNumber = -6;
	SET @PreviousTime = DATEADD(minute, @MinuteNumber * 60, @RequestedTime);
	-- SELECT
	SELECT *
	FROM vHD_NOVAON vN WITH (NOLOCK)
	WHERE CONVERT(DATETIME, vN.SIGNEDTIME, 103) BETWEEN @PreviousTime AND @RequestedTime
	AND DaXuatHD = 0
END
