USE [Saleman]
GO
/****** Object:  View [dbo].[vHD_NOVAON]    Script Date: 08/14/2020 08:16:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[vHD_NOVAON]
AS
SELECT 'HD' + PX.SCT AS PMSMAT_INVOICEID, 
		PX.Ngay AS INITTIME, -- ngày khởi tạo
		PX.NgayPH AS SIGNEDTIME, -- ngày phát hành
		KH.ID AS CUSTOMERCODE, 
		PX.TenKhach AS CUSBUYER, 
		KH.NAME AS CUSNAME, 
		PX.DiaChi AS CUSADDRESS, 
		KH.STK AS CUSACCOUNTNUMBER,                     
		'' AS TENNHKHACH, 
		KH.Email, 
		(
			CASE 
				WHEN PX.MSTHUE = '' THEN KH.MSTHUE 
				ELSE PX.MSTHUE 
			END
		) AS CUSTAXCODE, 
		NV.Name AS CUSPAYMENTMETHOD, 
		PX.ThueGTGT AS TAXRATE, 
		HH.NAME AS PRODUCTNAME, 
		(
			CASE 
				WHEN DHX.HS <= 1 OR DHX.Hs IS NULL THEN HH.TDVT 
				ELSE HH.TDVT1 
			END
		) AS QUANTITYUNIT, 
		DHX.SL AS QUANTITY, 
		DHX.DG AS RETAILPRICE, 
		DHX.SL * DHX.DG AS TOTALMONEY, 
		'VND' AS TienTe, 
		'' AS GhiChu, 
		DHX.ID AS STT, 
		PX.ID, 
		PX.DaXuatHD,
		PX.NoiDung AS NOTE

FROM	dbo.DonHangXuat AS DHX WITH (NOLOCK)
		LEFT OUTER JOIN dbo.PhieuXuat	AS PX WITH (NOLOCK) ON DHX.IDPX = PX.ID 
		LEFT OUTER JOIN dbo.KHACHHANG	AS KH WITH (NOLOCK) ON PX.IDKHACH = KH.ID 
		LEFT OUTER JOIN dbo.NghiepVu	AS NV WITH (NOLOCK) ON PX.MaNv = NV.ID 
		LEFT OUTER JOIN dbo.HANGHOA		AS HH WITH (NOLOCK) ON DHX.IDHANG = HH.ID
WHERE	PX.LOAICT = 'XK' AND PX.DaXuatHD = 0
