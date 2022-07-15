using System;

namespace DS.BusinessObject.Invoice.ReleaseDocument
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue", IsNullable = false)]
    public partial class HSoThueDTu
    {

        private HSoThueDTuHSoKhaiThue hSoKhaiThueField;

        /// <remarks/>
        public HSoThueDTuHSoKhaiThue HSoKhaiThue
        {
            get
            {
                return this.hSoKhaiThueField;
            }
            set
            {
                this.hSoKhaiThueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThue
    {

        private HSoThueDTuHSoKhaiThueTTinChung tTinChungField;

        private HSoThueDTuHSoKhaiThueCTieuTKhaiChinh cTieuTKhaiChinhField;

        private string idField;

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChung TTinChung
        {
            get
            {
                return this.tTinChungField;
            }
            set
            {
                this.tTinChungField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinh CTieuTKhaiChinh
        {
            get
            {
                return this.cTieuTKhaiChinhField;
            }
            set
            {
                this.cTieuTKhaiChinhField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChung
    {

        private HSoThueDTuHSoKhaiThueTTinChungTTinDVu tTinDVuField;

        private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue tTinTKhaiThueField;

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChungTTinDVu TTinDVu
        {
            get
            {
                return this.tTinDVuField;
            }
            set
            {
                this.tTinDVuField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue TTinTKhaiThue
        {
            get
            {
                return this.tTinTKhaiThueField;
            }
            set
            {
                this.tTinTKhaiThueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChungTTinDVu
    {

        private string maDVuField;

        private string tenDVuField;

        private string pbanDVuField;

        private string ttinNhaCCapDVuField;

        /// <remarks/>
        public string maDVu
        {
            get
            {
                return this.maDVuField;
            }
            set
            {
                this.maDVuField = value;
            }
        }

        /// <remarks/>
        public string tenDVu
        {
            get
            {
                return this.tenDVuField;
            }
            set
            {
                this.tenDVuField = value;
            }
        }

        /// <remarks/>
        public string pbanDVu
        {
            get
            {
                return this.pbanDVuField;
            }
            set
            {
                this.pbanDVuField = value;
            }
        }

        /// <remarks/>
        public string ttinNhaCCapDVu
        {
            get
            {
                return this.ttinNhaCCapDVuField;
            }
            set
            {
                this.ttinNhaCCapDVuField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue
    {

        private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue tKhaiThueField;

        private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT nNTField;

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue TKhaiThue
        {
            get
            {
                return this.tKhaiThueField;
            }
            set
            {
                this.tKhaiThueField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT NNT
        {
            get
            {
                return this.nNTField;
            }
            set
            {
                this.nNTField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue
    {

        private byte maTKhaiField;

        private string tenTKhaiField;

        private string moTaBMauField;

        private string pbanTKhaiXMLField;

        private string loaiTKhaiField;

        private byte soLanField;

        private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue kyKKhaiThueField;

        private string maCQTNoiNopField;

        private string tenCQTNoiNopField;

        private System.DateTime ngayLapTKhaiField;

        private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan giaHanField;

        private string nguoiKyField;

        private System.DateTime ngayKyField;

        private string nganhNgheKDField;

        /// <remarks/>
        public byte maTKhai
        {
            get
            {
                return this.maTKhaiField;
            }
            set
            {
                this.maTKhaiField = value;
            }
        }

        /// <remarks/>
        public string tenTKhai
        {
            get
            {
                return this.tenTKhaiField;
            }
            set
            {
                this.tenTKhaiField = value;
            }
        }

        /// <remarks/>
        public string moTaBMau
        {
            get
            {
                return this.moTaBMauField;
            }
            set
            {
                this.moTaBMauField = value;
            }
        }

        /// <remarks/>
        public string pbanTKhaiXML
        {
            get
            {
                return this.pbanTKhaiXMLField;
            }
            set
            {
                this.pbanTKhaiXMLField = value;
            }
        }

        /// <remarks/>
        public string loaiTKhai
        {
            get
            {
                return this.loaiTKhaiField;
            }
            set
            {
                this.loaiTKhaiField = value;
            }
        }

        /// <remarks/>
        public byte soLan
        {
            get
            {
                return this.soLanField;
            }
            set
            {
                this.soLanField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue KyKKhaiThue
        {
            get
            {
                return this.kyKKhaiThueField;
            }
            set
            {
                this.kyKKhaiThueField = value;
            }
        }

        /// <remarks/>
        public string maCQTNoiNop
        {
            get
            {
                return this.maCQTNoiNopField;
            }
            set
            {
                this.maCQTNoiNopField = value;
            }
        }

        /// <remarks/>
        public string tenCQTNoiNop
        {
            get
            {
                return this.tenCQTNoiNopField;
            }
            set
            {
                this.tenCQTNoiNopField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ngayLapTKhai
        {
            get
            {
                return this.ngayLapTKhaiField;
            }
            set
            {
                this.ngayLapTKhaiField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan GiaHan
        {
            get
            {
                return this.giaHanField;
            }
            set
            {
                this.giaHanField = value;
            }
        }

        /// <remarks/>
        public string nguoiKy
        {
            get
            {
                return this.nguoiKyField;
            }
            set
            {
                this.nguoiKyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ngayKy
        {
            get
            {
                return this.ngayKyField;
            }
            set
            {
                this.ngayKyField = value;
            }
        }

        /// <remarks/>
        public string nganhNgheKD
        {
            get
            {
                return this.nganhNgheKDField;
            }
            set
            {
                this.nganhNgheKDField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue
    {

        private string kieuKyField;

        private string kyKKhaiField;

        private string kyKKhaiTuNgayField;

        private string kyKKhaiDenNgayField;

        private string kyKKhaiTuThangField;

        private string kyKKhaiDenThangField;

        /// <remarks/>
        public string kieuKy
        {
            get
            {
                return this.kieuKyField;
            }
            set
            {
                this.kieuKyField = value;
            }
        }

        /// <remarks/>
        public string kyKKhai
        {
            get
            {
                return this.kyKKhaiField;
            }
            set
            {
                this.kyKKhaiField = value;
            }
        }

        /// <remarks/>
        public string kyKKhaiTuNgay
        {
            get
            {
                return this.kyKKhaiTuNgayField;
            }
            set
            {
                this.kyKKhaiTuNgayField = value;
            }
        }

        /// <remarks/>
        public string kyKKhaiDenNgay
        {
            get
            {
                return this.kyKKhaiDenNgayField;
            }
            set
            {
                this.kyKKhaiDenNgayField = value;
            }
        }

        /// <remarks/>
        public string kyKKhaiTuThang
        {
            get
            {
                return this.kyKKhaiTuThangField;
            }
            set
            {
                this.kyKKhaiTuThangField = value;
            }
        }

        /// <remarks/>
        public string kyKKhaiDenThang
        {
            get
            {
                return this.kyKKhaiDenThangField;
            }
            set
            {
                this.kyKKhaiDenThangField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan
    {

        private string maLyDoGiaHanField;

        private string lyDoGiaHanField;

        /// <remarks/>
        public string maLyDoGiaHan
        {
            get
            {
                return this.maLyDoGiaHanField;
            }
            set
            {
                this.maLyDoGiaHanField = value;
            }
        }

        /// <remarks/>
        public string lyDoGiaHan
        {
            get
            {
                return this.lyDoGiaHanField;
            }
            set
            {
                this.lyDoGiaHanField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT
    {

        private string mstField;

        private string tenNNTField;

        private string dchiNNTField;

        private string phuongXaField;

        private string maHuyenNNTField;

        private string tenHuyenNNTField;

        private string maTinhNNTField;

        private string tenTinhNNTField;

        private string dthoaiNNTField;

        private string faxNNTField;

        private string emailNNTField;

        /// <remarks/>
        public string mst
        {
            get
            {
                return this.mstField;
            }
            set
            {
                this.mstField = value;
            }
        }

        /// <remarks/>
        public string tenNNT
        {
            get
            {
                return this.tenNNTField;
            }
            set
            {
                this.tenNNTField = value;
            }
        }

        /// <remarks/>
        public string dchiNNT
        {
            get
            {
                return this.dchiNNTField;
            }
            set
            {
                this.dchiNNTField = value;
            }
        }

        /// <remarks/>
        public string phuongXa
        {
            get
            {
                return this.phuongXaField;
            }
            set
            {
                this.phuongXaField = value;
            }
        }

        /// <remarks/>
        public string maHuyenNNT
        {
            get
            {
                return this.maHuyenNNTField;
            }
            set
            {
                this.maHuyenNNTField = value;
            }
        }

        /// <remarks/>
        public string tenHuyenNNT
        {
            get
            {
                return this.tenHuyenNNTField;
            }
            set
            {
                this.tenHuyenNNTField = value;
            }
        }

        /// <remarks/>
        public string maTinhNNT
        {
            get
            {
                return this.maTinhNNTField;
            }
            set
            {
                this.maTinhNNTField = value;
            }
        }

        /// <remarks/>
        public string tenTinhNNT
        {
            get
            {
                return this.tenTinhNNTField;
            }
            set
            {
                this.tenTinhNNTField = value;
            }
        }

        /// <remarks/>
        public string dthoaiNNT
        {
            get
            {
                return this.dthoaiNNTField;
            }
            set
            {
                this.dthoaiNNTField = value;
            }
        }

        /// <remarks/>
        public string faxNNT
        {
            get
            {
                return this.faxNNTField;
            }
            set
            {
                this.faxNNTField = value;
            }
        }

        /// <remarks/>
        public string emailNNT
        {
            get
            {
                return this.emailNNTField;
            }
            set
            {
                this.emailNNTField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinh
    {

        private HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDon hoaDonField;

        private HSoThueDTuHSoKhaiThueCTieuTKhaiChinhDonViChuQuan donViChuQuanField;

        private string tenCQTTiepNhanField;

        private string nguoiDaiDienField;

        private System.DateTime ngayBCaoField;

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDon HoaDon
        {
            get
            {
                return this.hoaDonField;
            }
            set
            {
                this.hoaDonField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinhDonViChuQuan DonViChuQuan
        {
            get
            {
                return this.donViChuQuanField;
            }
            set
            {
                this.donViChuQuanField = value;
            }
        }

        /// <remarks/>
        public string tenCQTTiepNhan
        {
            get
            {
                return this.tenCQTTiepNhanField;
            }
            set
            {
                this.tenCQTTiepNhanField = value;
            }
        }

        /// <remarks/>
        public string nguoiDaiDien
        {
            get
            {
                return this.nguoiDaiDienField;
            }
            set
            {
                this.nguoiDaiDienField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ngayBCao
        {
            get
            {
                return this.ngayBCaoField;
            }
            set
            {
                this.ngayBCaoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDon
    {

        private HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTiet chiTietField;

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTiet ChiTiet
        {
            get
            {
                return this.chiTietField;
            }
            set
            {
                this.chiTietField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTiet
    {

        private string tenLoaiHDonField;

        private string mauSoField;

        private string kyHieuField;

        private long soLuongField;

        private string tuSoField;

        private string denSoField;

        private System.DateTime ngayBDauSDungField;

        private HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietDoanhNghiepIn doanhNghiepInField;

        private HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietHopDongDatIn hopDongDatInField;

        private string idField;

        /// <remarks/>
        public string tenLoaiHDon
        {
            get
            {
                return this.tenLoaiHDonField;
            }
            set
            {
                this.tenLoaiHDonField = value;
            }
        }

        /// <remarks/>
        public string mauSo
        {
            get
            {
                return this.mauSoField;
            }
            set
            {
                this.mauSoField = value;
            }
        }

        /// <remarks/>
        public string kyHieu
        {
            get
            {
                return this.kyHieuField;
            }
            set
            {
                this.kyHieuField = value;
            }
        }

        /// <remarks/>
        public long soLuong
        {
            get
            {
                return this.soLuongField;
            }
            set
            {
                this.soLuongField = value;
            }
        }

        /// <remarks/>
        public string tuSo
        {
            get
            {
                return this.tuSoField;
            }
            set
            {
                this.tuSoField = value;
            }
        }

        /// <remarks/>
        public string denSo
        {
            get
            {
                return this.denSoField;
            }
            set
            {
                this.denSoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime ngayBDauSDung
        {
            get
            {
                return this.ngayBDauSDungField;
            }
            set
            {
                this.ngayBDauSDungField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietDoanhNghiepIn DoanhNghiepIn
        {
            get
            {
                return this.doanhNghiepInField;
            }
            set
            {
                this.doanhNghiepInField = value;
            }
        }

        /// <remarks/>
        public HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietHopDongDatIn HopDongDatIn
        {
            get
            {
                return this.hopDongDatInField;
            }
            set
            {
                this.hopDongDatInField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietDoanhNghiepIn
    {

        private string tenField;

        private string mstField;

        /// <remarks/>
        public string ten
        {
            get
            {
                return this.tenField;
            }
            set
            {
                this.tenField = value;
            }
        }

        /// <remarks/>
        public string mst
        {
            get
            {
                return this.mstField;
            }
            set
            {
                this.mstField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDonChiTietHopDongDatIn
    {

        private string soField;

        private string ngayField;

        /// <remarks/>
        public string so
        {
            get
            {
                return this.soField;
            }
            set
            {
                this.soField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string ngay
        {
            get
            {
                return this.ngayField;
            }
            set
            {
                this.ngayField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
    public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinhDonViChuQuan
    {

        private string tenField;

        private string mstField;

        /// <remarks/>
        public string ten
        {
            get
            {
                return this.tenField;
            }
            set
            {
                this.tenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string mst
        {
            get
            {
                return this.mstField;
            }
            set
            {
                this.mstField = value;
            }
        }
    }
}
