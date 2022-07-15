USE [SALE_THUE]
GO
/****** Object:  StoredProcedure [dbo].[USP_NOVAON_UPDATE_INVOICES]    Script Date: 09/24/2020 08:49:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		TuyenNV
-- Create date: 12/08/2020
-- Description:	Cập nhật thông tin hóa đơn sau khi xuất
-- =============================================
CREATE PROCEDURE [dbo].[USP_NOVAON_UPDATE_INVOICES]
	@PMSMAT_INVOICEID NVARCHAR(4000)
AS
BEGIN	
	SET NOCOUNT OFF;
	DECLARE @strOutput NVARCHAR(4000);
	set @strOutput = [dbo].Unicode2TCVN(@PMSMAT_INVOICEID);

	insert into dbo.logs(logtext) values (@PMSMAT_INVOICEID);

	UPDATE	dbo.PhieuXuat
	SET		dbo.PhieuXuat.DaXuatHD  = 1
	from	dbo.PhieuXuat
	WHERE	dbo.PhieuXuat.SCT IN (SELECT * FROM [dbo].[UDP_NOVAON_Split_String](@strOutput, ','));
END



