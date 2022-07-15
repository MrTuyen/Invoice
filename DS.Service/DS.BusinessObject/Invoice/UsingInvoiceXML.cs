using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Invoice
{
    public class UsingInvoiceXML
    {
        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue",
            IsNullable = false)]
        public partial class HSoThueDTu
        {

            private HSoThueDTuHSoKhaiThue hSoKhaiThueField;

            /// <remarks/>
            public HSoThueDTuHSoKhaiThue HSoKhaiThue
            {
                get { return this.hSoKhaiThueField; }
                set { this.hSoKhaiThueField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThue
        {

            private HSoThueDTuHSoKhaiThueTTinChung tTinChungField;

            private HSoThueDTuHSoKhaiThueCTieuTKhaiChinh cTieuTKhaiChinhField;

            private string idField;

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChung TTinChung
            {
                get { return this.tTinChungField; }
                set { this.tTinChungField = value; }
            }

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueCTieuTKhaiChinh CTieuTKhaiChinh
            {
                get { return this.cTieuTKhaiChinhField; }
                set { this.cTieuTKhaiChinhField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string id
            {
                get { return this.idField; }
                set { this.idField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThueTTinChung
        {

            private HSoThueDTuHSoKhaiThueTTinChungTTinDVu tTinDVuField;

            private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue tTinTKhaiThueField;

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChungTTinDVu TTinDVu
            {
                get { return this.tTinDVuField; }
                set { this.tTinDVuField = value; }
            }

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue TTinTKhaiThue
            {
                get { return this.tTinTKhaiThueField; }
                set { this.tTinTKhaiThueField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThueTTinChungTTinDVu
        {

            private string maDVuField;

            private string tenDVuField;

            private string pbanDVuField;

            private string ttinNhaCCapDVuField;

            /// <remarks/>
            public string maDVu
            {
                get { return this.maDVuField; }
                set { this.maDVuField = value; }
            }

            /// <remarks/>
            public string tenDVu
            {
                get { return this.tenDVuField; }
                set { this.tenDVuField = value; }
            }

            /// <remarks/>
            public string pbanDVu
            {
                get { return this.pbanDVuField; }
                set { this.pbanDVuField = value; }
            }

            /// <remarks/>
            public string ttinNhaCCapDVu
            {
                get { return this.ttinNhaCCapDVuField; }
                set { this.ttinNhaCCapDVuField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThue
        {

            private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue tKhaiThueField;

            private HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT nNTField;

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThue TKhaiThue
            {
                get { return this.tKhaiThueField; }
                set { this.tKhaiThueField = value; }
            }

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueNNT NNT
            {
                get { return this.nNTField; }
                set { this.nNTField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
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
                get { return this.maTKhaiField; }
                set { this.maTKhaiField = value; }
            }

            /// <remarks/>
            public string tenTKhai
            {
                get { return this.tenTKhaiField; }
                set { this.tenTKhaiField = value; }
            }

            /// <remarks/>
            public string moTaBMau
            {
                get { return this.moTaBMauField; }
                set { this.moTaBMauField = value; }
            }

            /// <remarks/>
            public string pbanTKhaiXML
            {
                get { return this.pbanTKhaiXMLField; }
                set { this.pbanTKhaiXMLField = value; }
            }

            /// <remarks/>
            public string loaiTKhai
            {
                get { return this.loaiTKhaiField; }
                set { this.loaiTKhaiField = value; }
            }

            /// <remarks/>
            public byte soLan
            {
                get { return this.soLanField; }
                set { this.soLanField = value; }
            }

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueKyKKhaiThue KyKKhaiThue
            {
                get { return this.kyKKhaiThueField; }
                set { this.kyKKhaiThueField = value; }
            }

            /// <remarks/>
            public string maCQTNoiNop
            {
                get { return this.maCQTNoiNopField; }
                set { this.maCQTNoiNopField = value; }
            }

            /// <remarks/>
            public string tenCQTNoiNop
            {
                get { return this.tenCQTNoiNopField; }
                set { this.tenCQTNoiNopField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime ngayLapTKhai
            {
                get { return this.ngayLapTKhaiField; }
                set { this.ngayLapTKhaiField = value; }
            }

            /// <remarks/>
            public HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan GiaHan
            {
                get { return this.giaHanField; }
                set { this.giaHanField = value; }
            }

            /// <remarks/>
            public string nguoiKy
            {
                get { return this.nguoiKyField; }
                set { this.nguoiKyField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public System.DateTime ngayKy
            {
                get { return this.ngayKyField; }
                set { this.ngayKyField = value; }
            }

            /// <remarks/>
            public string nganhNgheKD
            {
                get { return this.nganhNgheKDField; }
                set { this.nganhNgheKDField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
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
                get { return this.kieuKyField; }
                set { this.kieuKyField = value; }
            }

            /// <remarks/>
            public string kyKKhai
            {
                get { return this.kyKKhaiField; }
                set { this.kyKKhaiField = value; }
            }

            /// <remarks/>
            public string kyKKhaiTuNgay
            {
                get { return this.kyKKhaiTuNgayField; }
                set { this.kyKKhaiTuNgayField = value; }
            }

            /// <remarks/>
            public string kyKKhaiDenNgay
            {
                get { return this.kyKKhaiDenNgayField; }
                set { this.kyKKhaiDenNgayField = value; }
            }

            /// <remarks/>
            public string kyKKhaiTuThang
            {
                get { return this.kyKKhaiTuThangField; }
                set { this.kyKKhaiTuThangField = value; }
            }

            /// <remarks/>
            public string kyKKhaiDenThang
            {
                get { return this.kyKKhaiDenThangField; }
                set { this.kyKKhaiDenThangField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThueTTinChungTTinTKhaiThueTKhaiThueGiaHan
        {

            private string maLyDoGiaHanField;

            private string lyDoGiaHanField;

            /// <remarks/>
            public string maLyDoGiaHan
            {
                get { return this.maLyDoGiaHanField; }
                set { this.maLyDoGiaHanField = value; }
            }

            /// <remarks/>
            public string lyDoGiaHan
            {
                get { return this.lyDoGiaHanField; }
                set { this.lyDoGiaHanField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
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
                get { return this.mstField; }
                set { this.mstField = value; }
            }

            /// <remarks/>
            public string tenNNT
            {
                get { return this.tenNNTField; }
                set { this.tenNNTField = value; }
            }

            /// <remarks/>
            public string dchiNNT
            {
                get { return this.dchiNNTField; }
                set { this.dchiNNTField = value; }
            }

            /// <remarks/>
            public string phuongXa
            {
                get { return this.phuongXaField; }
                set { this.phuongXaField = value; }
            }

            /// <remarks/>
            public string maHuyenNNT
            {
                get { return this.maHuyenNNTField; }
                set { this.maHuyenNNTField = value; }
            }

            /// <remarks/>
            public string tenHuyenNNT
            {
                get { return this.tenHuyenNNTField; }
                set { this.tenHuyenNNTField = value; }
            }

            /// <remarks/>
            public string maTinhNNT
            {
                get { return this.maTinhNNTField; }
                set { this.maTinhNNTField = value; }
            }

            /// <remarks/>
            public string tenTinhNNT
            {
                get { return this.tenTinhNNTField; }
                set { this.tenTinhNNTField = value; }
            }

            /// <remarks/>
            public string dthoaiNNT
            {
                get { return this.dthoaiNNTField; }
                set { this.dthoaiNNTField = value; }
            }

            /// <remarks/>
            public string faxNNT
            {
                get { return this.faxNNTField; }
                set { this.faxNNTField = value; }
            }

            /// <remarks/>
            public string emailNNT
            {
                get { return this.emailNNTField; }
                set { this.emailNNTField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinh
        {

            private byte kyBCaoCuoiField;

            private byte chuyenDiaDiemField;

            private DateTime ngayDauKyBCField;

            private DateTime ngayCuoiKyBCField;

            private List<ChiTiet> hoaDonField;

            private long tongCongSoTonDKyField;

            private long tongCongSDungField;

            private long tongCongSoTonCKyField;

            private string nguoiLapBieuField;

            private string nguoiDaiDienField;

            private System.DateTime ngayBCaoField;

            /// <remarks/>
            public byte kyBCaoCuoi
            {
                get { return this.kyBCaoCuoiField; }
                set { this.kyBCaoCuoiField = value; }
            }

            /// <remarks/>
            public byte chuyenDiaDiem
            {
                get { return this.chuyenDiaDiemField; }
                set { this.chuyenDiaDiemField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public DateTime ngayDauKyBC
            {
                get { return this.ngayDauKyBCField; }
                set { this.ngayDauKyBCField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public DateTime ngayCuoiKyBC
            {
                get { return this.ngayCuoiKyBCField; }
                set { this.ngayCuoiKyBCField = value; }
            }

            /// <remarks/>
            public List<ChiTiet> HoaDon
            {
                get { return this.hoaDonField; }
                set { this.hoaDonField = value; }
            }

            /// <remarks/>
            public long tongCongSoTonDKy
            {
                get { return this.tongCongSoTonDKyField; }
                set { this.tongCongSoTonDKyField = value; }
            }

            /// <remarks/>
            public long tongCongSDung
            {
                get { return this.tongCongSDungField; }
                set { this.tongCongSDungField = value; }
            }

            /// <remarks/>
            public long tongCongSoTonCKy
            {
                get { return this.tongCongSoTonCKyField; }
                set { this.tongCongSoTonCKyField = value; }
            }

            /// <remarks/>
            public string nguoiLapBieu
            {
                get { return this.nguoiLapBieuField; }
                set { this.nguoiLapBieuField = value; }
            }

            /// <remarks/>
            public string nguoiDaiDien
            {
                get { return this.nguoiDaiDienField; }
                set { this.nguoiDaiDienField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
            public DateTime ngayBCao
            {
                get { return this.ngayBCaoField; }
                set { this.ngayBCaoField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class HSoThueDTuHSoKhaiThueCTieuTKhaiChinhHoaDon
        {
            private List<ChiTiet> chiTietField;

            /// <remarks/>
            public List<ChiTiet> ChiTiet
            {
                get { return this.chiTietField; }
                set { this.chiTietField = value; }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
            Namespace = "http://kekhaithue.gdt.gov.vn/TKhaiThue")]
        public partial class ChiTiet
        {

            private string maHoaDonField;

            private string tenHDonField;

            private string kHieuMauHDonField;

            private string kHieuHDonField;

            private long soTonMuaTrKy_tongSoField;

            private string soTonDauKy_tuSoField;

            private string soTonDauKy_denSoField;

            private string muaTrongKy_tuSoField;

            private string muaTrongKy_denSoField;

            private string tongSoSuDung_tuSoField;

            private string tongSoSuDung_denSoField;

            private long tongSoSuDung_congField;

            private long soDaSDungField;

            private long xoaBo_soLuongField;

            private string xoaBo_soField;

            private int mat_soLuongField;

            private string mat_soField;

            private long huy_soLuongField;

            private string huy_soField;

            private string tonCuoiKy_tuSoField;

            private string tonCuoiKy_denSoField;

            private long tonCuoiKy_soLuongField;

            private string idField;

            /// <remarks/>
            public string maHoaDon
            {
                get { return this.maHoaDonField; }
                set { this.maHoaDonField = value; }
            }

            /// <remarks/>
            public string tenHDon
            {
                get { return this.tenHDonField; }
                set { this.tenHDonField = value; }
            }

            /// <remarks/>
            public string kHieuMauHDon
            {
                get { return this.kHieuMauHDonField; }
                set { this.kHieuMauHDonField = value; }
            }

            /// <remarks/>
            public string kHieuHDon
            {
                get { return this.kHieuHDonField; }
                set { this.kHieuHDonField = value; }
            }

            /// <remarks/>
            public long soTonMuaTrKy_tongSo
            {
                get { return this.soTonMuaTrKy_tongSoField; }
                set { this.soTonMuaTrKy_tongSoField = value; }
            }

            /// <remarks/>
            public string soTonDauKy_tuSo
            {
                get { return this.soTonDauKy_tuSoField; }
                set { this.soTonDauKy_tuSoField = value; }
            }

            /// <remarks/>
            public string soTonDauKy_denSo
            {
                get { return this.soTonDauKy_denSoField; }
                set { this.soTonDauKy_denSoField = value; }
            }

            /// <remarks/>
            public string muaTrongKy_tuSo
            {
                get { return this.muaTrongKy_tuSoField; }
                set { this.muaTrongKy_tuSoField = value; }
            }

            /// <remarks/>
            public string muaTrongKy_denSo
            {
                get { return this.muaTrongKy_denSoField; }
                set { this.muaTrongKy_denSoField = value; }
            }

            /// <remarks/>
            public string tongSoSuDung_tuSo
            {
                get { return this.tongSoSuDung_tuSoField; }
                set { this.tongSoSuDung_tuSoField = value; }
            }

            /// <remarks/>
            public string tongSoSuDung_denSo
            {
                get { return this.tongSoSuDung_denSoField; }
                set { this.tongSoSuDung_denSoField = value; }
            }

            /// <remarks/>
            public long tongSoSuDung_cong
            {
                get { return this.tongSoSuDung_congField; }
                set { this.tongSoSuDung_congField = value; }
            }

            /// <remarks/>
            public long soDaSDung
            {
                get { return this.soDaSDungField; }
                set { this.soDaSDungField = value; }
            }

            /// <remarks/>
            public long xoaBo_soLuong
            {
                get { return this.xoaBo_soLuongField; }
                set { this.xoaBo_soLuongField = value; }
            }

            /// <remarks/>
            public string xoaBo_so
            {
                get { return this.xoaBo_soField; }
                set { this.xoaBo_soField = value; }
            }

            /// <remarks/>
            public int mat_soLuong
            {
                get { return this.mat_soLuongField; }
                set { this.mat_soLuongField = value; }
            }

            /// <remarks/>
            public string mat_so
            {
                get { return this.mat_soField; }
                set { this.mat_soField = value; }
            }

            /// <remarks/>
            public long huy_soLuong
            {
                get { return this.huy_soLuongField; }
                set { this.huy_soLuongField = value; }
            }

            /// <remarks/>
            public string huy_so
            {
                get { return this.huy_soField; }
                set { this.huy_soField = value; }
            }

            /// <remarks/>
            public string tonCuoiKy_tuSo
            {
                get { return this.tonCuoiKy_tuSoField; }
                set { this.tonCuoiKy_tuSoField = value; }
            }

            /// <remarks/>
            public string tonCuoiKy_denSo
            {
                get { return this.tonCuoiKy_denSoField; }
                set { this.tonCuoiKy_denSoField = value; }
            }

            /// <remarks/>
            public long tonCuoiKy_soLuong
            {
                get { return this.tonCuoiKy_soLuongField; }
                set { this.tonCuoiKy_soLuongField = value; }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string id
            {
                get { return this.idField; }
                set { this.idField = value; }
            }
        }
    }
}
