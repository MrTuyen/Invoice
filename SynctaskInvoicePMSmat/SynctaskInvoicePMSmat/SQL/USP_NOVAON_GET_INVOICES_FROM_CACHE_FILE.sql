USE [SALE_THUE]
GO
/****** Object:  StoredProcedure [dbo].[USP_NOVAON_GET_INVOICES_FROM_CACHE_FILE]    Script Date: 09/24/2020 08:48:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		TuyenNV
-- Create date: 15/09/2020
-- Description:	Lấy những bản ghi từ cache file
-- =============================================
CREATE PROCEDURE [dbo].[USP_NOVAON_GET_INVOICES_FROM_CACHE_FILE]
	@ListInvoiceIds VARCHAR(8000)
AS
BEGIN	
	SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
	SET NOCOUNT ON;

	-- SELECT
	SELECT *
	FROM vHD_NOVAON vN WITH (NOLOCK)
	WHERE vN.PMSMAT_INVOICEID IN (SELECT * FROM [dbo].[UDP_NOVAON_Split_String](@ListInvoiceIds, ','))
END
