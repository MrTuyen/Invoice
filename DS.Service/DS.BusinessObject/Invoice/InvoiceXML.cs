using System.Collections.Generic;

namespace DS.BusinessObject.Invoice
{
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1", IsNullable = false)]
    public partial class invoice
    {

        private invoiceInvoiceData invoiceDataField;

        private invoiceControlData controlDataField;

        private Signature signatureField;

        /// <remarks/>
        public invoiceInvoiceData invoiceData
        {
            get { return this.invoiceDataField; }
            set { this.invoiceDataField = value; }
        }

        /// <remarks/>
        public invoiceControlData controlData
        {
            get { return this.controlDataField; }
            set { this.controlDataField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature
        {
            get { return this.signatureField; }
            set { this.signatureField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceData
    {

        private string invoiceTypeField;

        private string templateCodeField;

        private string invoiceSeriesField;

        private string invoiceNumberField;

        private string invoiceNameField;

        private System.DateTime invoiceIssuedDateField;

        private System.DateTime signedDateField;

        private string currencyCodeField;

        private int adjustmentTypeField;

        private invoiceInvoiceDataPayments paymentsField;

        private string deliveryField;

        private string sellerLegalNameField;

        private string sellerTaxCodeField;

        private string sellerAddressLineField;

        private string sellerPhoneNumberField;

        private string sellerFaxNumberField;

        private string sellerEmailField;

        private string sellerBankAccountField;

        private string sellerBankNameField;

        private string buyerLegalNameField;

        private string buyerTaxCodeField;

        private string buyerAddressLineField;

        private System.DateTime deliveryOrderDateField;
        #region Phiếu xuất kho
        private string deliveryOrderByField;

        private string deliveryByField;

        private string fromWarehouseNameField;

        private string toWarehouseNameField;

        private string transportationMethodField;

        private string contractNumberField;

        private string deliveryOrderContentField;
        #endregion

        private decimal exchangeRateField;

        private invoiceInvoiceDataItems itemsField;

        private invoiceInvoiceDataInvoiceTaxBreakdowns invoiceTaxBreakdownsField;

        private decimal totalAmountWithoutVATField;

        private decimal totalVATAmountField;

        private decimal totalProtectEnvironmentTaxField;

        private decimal totalOtherTaxesChargesField;

        private decimal totalRefundFeeField;

        private decimal totalServiceFeeField;

        private decimal totalAmountWithVATField;

        private string totalAmountWithVATInWordsField;

        private string invoiceNoteField;

        private decimal discountAmountField;

        private byte isDiscountAmtPosField;

        private decimal totalAmountWithVATFrnField;

        private string userDefinesField;

        private string idField;

        /// <remarks/>
        public string invoiceType
        {
            get { return this.invoiceTypeField; }
            set { this.invoiceTypeField = value; }
        }

        /// <remarks/>
        public string templateCode
        {
            get { return this.templateCodeField; }
            set { this.templateCodeField = value; }
        }

        /// <remarks/>
        public string invoiceSeries
        {
            get { return this.invoiceSeriesField; }
            set { this.invoiceSeriesField = value; }
        }

        /// <remarks/>
        public string invoiceNumber
        {
            get { return this.invoiceNumberField; }
            set { this.invoiceNumberField = value; }
        }

        /// <remarks/>
        public string invoiceName
        {
            get { return this.invoiceNameField; }
            set { this.invoiceNameField = value; }
        }

        /// <remarks/>
        public System.DateTime invoiceIssuedDate
        {
            get { return this.invoiceIssuedDateField; }
            set { this.invoiceIssuedDateField = value; }
        }

        /// <remarks/>
        public System.DateTime signedDate
        {
            get { return this.signedDateField; }
            set { this.signedDateField = value; }
        }

        /// <remarks/>
        public string currencyCode
        {
            get { return this.currencyCodeField; }
            set { this.currencyCodeField = value; }
        }

        /// <remarks/>
        public int adjustmentType
        {
            get { return this.adjustmentTypeField; }
            set { this.adjustmentTypeField = value; }
        }

        /// <remarks/>
        public invoiceInvoiceDataPayments payments
        {
            get { return this.paymentsField; }
            set { this.paymentsField = value; }
        }

        /// <remarks/>
        public string delivery
        {
            get { return this.deliveryField; }
            set { this.deliveryField = value; }
        }

        /// <remarks/>
        public string sellerLegalName
        {
            get { return this.sellerLegalNameField; }
            set { this.sellerLegalNameField = value; }
        }

        /// <remarks/>
        public string sellerTaxCode
        {
            get { return this.sellerTaxCodeField; }
            set { this.sellerTaxCodeField = value; }
        }

        /// <remarks/>
        public string sellerAddressLine
        {
            get { return this.sellerAddressLineField; }
            set { this.sellerAddressLineField = value; }
        }

        /// <remarks/>
        public string sellerPhoneNumber
        {
            get { return this.sellerPhoneNumberField; }
            set { this.sellerPhoneNumberField = value; }
        }

        /// <remarks/>
        public string sellerFaxNumber
        {
            get { return this.sellerFaxNumberField; }
            set { this.sellerFaxNumberField = value; }
        }

        /// <remarks/>
        public string sellerEmail
        {
            get { return this.sellerEmailField; }
            set { this.sellerEmailField = value; }
        }

        /// <remarks/>
        public string sellerBankAccount
        {
            get { return this.sellerBankAccountField; }
            set { this.sellerBankAccountField = value; }
        }

        /// <remarks/>
        public string sellerBankName
        {
            get { return this.sellerBankNameField; }
            set { this.sellerBankNameField = value; }
        }

        /// <remarks/>
        public string buyerLegalName
        {
            get { return this.buyerLegalNameField; }
            set { this.buyerLegalNameField = value; }
        }

        /// <remarks/>
        public string buyerTaxCode
        {
            get { return this.buyerTaxCodeField; }
            set { this.buyerTaxCodeField = value; }
        }

        /// <remarks/>
        public string buyerAddressLine
        {
            get { return this.buyerAddressLineField; }
            set { this.buyerAddressLineField = value; }
        }

        /// <remarks/>
        public System.DateTime deliveryOrderDate
        {
            get { return this.deliveryOrderDateField; }
            set { this.deliveryOrderDateField = value; }
        }

        #region Phiếu xuất kho
        public string deliveryOrderBy
        {
            get { return this.deliveryOrderByField; }
            set { this.deliveryOrderByField = value; }
        }

        public string deliveryBy
        {
            get { return this.deliveryByField; }
            set { this.deliveryByField = value; }
        }

        public string fromWarehouseName
        {
            get { return this.fromWarehouseNameField; }
            set { this.fromWarehouseNameField = value; }
        }

        public string toWarehouseName
        {
            get { return this.toWarehouseNameField; }
            set { this.toWarehouseNameField = value; }
        }

        public string transportationMethod
        {
            get { return this.transportationMethodField; }
            set { this.transportationMethodField = value; }
        }

        public string contractNumber
        {
            get { return this.contractNumberField; }
            set { this.contractNumberField = value; }
        }

        public string deliveryOrderContent
        {
            get { return this.deliveryOrderContentField; }
            set { this.deliveryOrderContentField = value; }
        }
        #endregion

        /// <remarks/>
        public decimal exchangeRate
        {
            get { return this.exchangeRateField; }
            set { this.exchangeRateField = value; }
        }

        /// <remarks/>
        public invoiceInvoiceDataItems items
        {
            get { return this.itemsField; }
            set { this.itemsField = value; }
        }

        /// <remarks/>
        public invoiceInvoiceDataInvoiceTaxBreakdowns invoiceTaxBreakdowns
        {
            get { return this.invoiceTaxBreakdownsField; }
            set { this.invoiceTaxBreakdownsField = value; }
        }

        /// <remarks/>
        public decimal totalAmountWithoutVAT
        {
            get { return this.totalAmountWithoutVATField; }
            set { this.totalAmountWithoutVATField = value; }
        }

        /// <remarks/>
        public decimal totalVATAmount
        {
            get { return this.totalVATAmountField; }
            set { this.totalVATAmountField = value; }
        }

        /// <remarks/>
        public decimal totalAmountWithVAT
        {
            get { return this.totalAmountWithVATField; }
            set { this.totalAmountWithVATField = value; }
        }

        /// <remarks/>
        public decimal totalEnvironmentProtectTax
        {
            get { return this.totalProtectEnvironmentTaxField; }
            set { this.totalProtectEnvironmentTaxField = value; }
        }
        public decimal totalOtherTaxesCharges
        {
            get { return this.totalOtherTaxesChargesField; }
            set { this.totalOtherTaxesChargesField = value; }
        }
        public decimal totalRefundFee
        {
            get { return this.totalRefundFeeField; }
            set { this.totalRefundFeeField = value; }
        }

        public decimal totalServiceFee
        {
            get { return this.totalServiceFeeField; }
            set { this.totalServiceFeeField = value; }
        }

        /// <remarks/>
        public string totalAmountWithVATInWords
        {
            get { return this.totalAmountWithVATInWordsField; }
            set { this.totalAmountWithVATInWordsField = value; }
        }

        /// <remarks/>
        public string invoiceNotes
        {
            get { return this.invoiceNoteField; }
            set { this.invoiceNoteField = value; }
        }
        
        /// <remarks/>
        public decimal discountAmount
        {
            get { return this.discountAmountField; }
            set { this.discountAmountField = value; }
        }

        /// <remarks/>
        public byte isDiscountAmtPos
        {
            get { return this.isDiscountAmtPosField; }
            set { this.isDiscountAmtPosField = value; }
        }

        /// <remarks/>
        public decimal totalAmountWithVATFrn
        {
            get { return this.totalAmountWithVATFrnField; }
            set { this.totalAmountWithVATFrnField = value; }
        }

        /// <remarks/>
        public string userDefines
        {
            get { return this.userDefinesField; }
            set { this.userDefinesField = value; }
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
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceDataPayments
    {

        private invoiceInvoiceDataPaymentsPayment paymentField;

        /// <remarks/>
        public invoiceInvoiceDataPaymentsPayment payment
        {
            get { return this.paymentField; }
            set { this.paymentField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceDataPaymentsPayment
    {

        private string paymentMethodNameField;

        /// <remarks/>
        public string paymentMethodName
        {
            get { return this.paymentMethodNameField; }
            set { this.paymentMethodNameField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceDataItems
    {

        private List<invoiceInvoiceDataItemsItem> itemField;

        /// <remarks/>
        public List<invoiceInvoiceDataItemsItem> item
        {
            get { return this.itemField; }
            set { this.itemField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceDataItemsItem
    {

        private uint lineNumberField;

        private string itemCodeField;

        private string itemNameField;

        private string unitNameField;

        private decimal quantityField;

        private decimal discountPercentageField;

        private decimal discountAmountField;

        private decimal itemTotalAmountWithoutVatField;

        private int vatPercentageField;

        private decimal vatAmountField;

        private decimal unitPriceField;

        private bool promotionField;

        /// <remarks/>
        public uint lineNumber
        {
            get { return this.lineNumberField; }
            set { this.lineNumberField = value; }
        }

        /// <remarks/>
        public string itemCode
        {
            get { return this.itemCodeField; }
            set { this.itemCodeField = value; }
        }

        /// <remarks/>
        public string itemName
        {
            get { return this.itemNameField; }
            set { this.itemNameField = value; }
        }

        /// <remarks/>
        public string unitName
        {
            get { return this.unitNameField; }
            set { this.unitNameField = value; }
        }

        /// <remarks/>
        public decimal quantity
        {
            get { return this.quantityField; }
            set { this.quantityField = value; }
        }

        /// <remarks/>
        public decimal discountPercentage
        {
            get { return this.discountPercentageField; }
            set { this.discountPercentageField = value; }
        }

        /// <remarks/>
        public decimal discountAmount
        {
            get { return this.discountAmountField; }
            set { this.discountAmountField = value; }
        }

        /// <remarks/>
        public decimal itemTotalAmountWithoutVat
        {
            get { return this.itemTotalAmountWithoutVatField; }
            set { this.itemTotalAmountWithoutVatField = value; }
        }

        /// <remarks/>
        public int vatPercentage
        {
            get { return this.vatPercentageField; }
            set { this.vatPercentageField = value; }
        }

        /// <remarks/>
        public decimal vatAmount
        {
            get { return this.vatAmountField; }
            set { this.vatAmountField = value; }
        }

        /// <remarks/>
        public decimal unitPrice
        {
            get { return this.unitPriceField; }
            set { this.unitPriceField = value; }
        }

        /// <remarks/>
        public bool promotion
        {
            get { return this.promotionField; }
            set { this.promotionField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceDataInvoiceTaxBreakdowns
    {

        private invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown invoiceTaxBreakdownField;

        /// <remarks/>
        public invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown invoiceTaxBreakdown
        {
            get { return this.invoiceTaxBreakdownField; }
            set { this.invoiceTaxBreakdownField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceInvoiceDataInvoiceTaxBreakdownsInvoiceTaxBreakdown
    {

        private byte vatPercentageField;

        /// <remarks/>
        public byte vatPercentage
        {
            get { return this.vatPercentageField; }
            set { this.vatPercentageField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "http://laphoadon.gdt.gov.vn/2014/09/invoicexml/v1")]
    public partial class invoiceControlData
    {

        private string systemCodeField;

        /// <remarks/>
        public string systemCode
        {
            get { return this.systemCodeField; }
            set { this.systemCodeField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2000/09/xmldsig#", IsNullable = false)]
    public partial class Signature
    {

        private SignatureSignedInfo signedInfoField;

        private string signatureValueField;

        private SignatureKeyInfo keyInfoField;

        private string idField;

        /// <remarks/>
        public SignatureSignedInfo SignedInfo
        {
            get { return this.signedInfoField; }
            set { this.signedInfoField = value; }
        }

        /// <remarks/>
        public string SignatureValue
        {
            get { return this.signatureValueField; }
            set { this.signatureValueField = value; }
        }

        /// <remarks/>
        public SignatureKeyInfo KeyInfo
        {
            get { return this.keyInfoField; }
            set { this.keyInfoField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id
        {
            get { return this.idField; }
            set { this.idField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfo
    {

        private SignatureSignedInfoCanonicalizationMethod canonicalizationMethodField;

        private SignatureSignedInfoSignatureMethod signatureMethodField;

        private SignatureSignedInfoReference referenceField;

        /// <remarks/>
        public SignatureSignedInfoCanonicalizationMethod CanonicalizationMethod
        {
            get { return this.canonicalizationMethodField; }
            set { this.canonicalizationMethodField = value; }
        }

        /// <remarks/>
        public SignatureSignedInfoSignatureMethod SignatureMethod
        {
            get { return this.signatureMethodField; }
            set { this.signatureMethodField = value; }
        }

        /// <remarks/>
        public SignatureSignedInfoReference Reference
        {
            get { return this.referenceField; }
            set { this.referenceField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoCanonicalizationMethod
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get { return this.algorithmField; }
            set { this.algorithmField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoSignatureMethod
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get { return this.algorithmField; }
            set { this.algorithmField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReference
    {

        private SignatureSignedInfoReferenceTransforms transformsField;

        private SignatureSignedInfoReferenceDigestMethod digestMethodField;

        private string digestValueField;

        private string uRIField;

        /// <remarks/>
        public SignatureSignedInfoReferenceTransforms Transforms
        {
            get { return this.transformsField; }
            set { this.transformsField = value; }
        }

        /// <remarks/>
        public SignatureSignedInfoReferenceDigestMethod DigestMethod
        {
            get { return this.digestMethodField; }
            set { this.digestMethodField = value; }
        }

        /// <remarks/>
        public string DigestValue
        {
            get { return this.digestValueField; }
            set { this.digestValueField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string URI
        {
            get { return this.uRIField; }
            set { this.uRIField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceTransforms
    {

        private SignatureSignedInfoReferenceTransformsTransform transformField;

        /// <remarks/>
        public SignatureSignedInfoReferenceTransformsTransform Transform
        {
            get { return this.transformField; }
            set { this.transformField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceTransformsTransform
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get { return this.algorithmField; }
            set { this.algorithmField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureSignedInfoReferenceDigestMethod
    {

        private string algorithmField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Algorithm
        {
            get { return this.algorithmField; }
            set { this.algorithmField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfo
    {

        private SignatureKeyInfoX509Data x509DataField;

        /// <remarks/>
        public SignatureKeyInfoX509Data X509Data
        {
            get { return this.x509DataField; }
            set { this.x509DataField = value; }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public partial class SignatureKeyInfoX509Data
    {

        private string x509SubjectNameField;

        private string x509CertificateField;

        /// <remarks/>
        public string X509SubjectName
        {
            get { return this.x509SubjectNameField; }
            set { this.x509SubjectNameField = value; }
        }

        /// <remarks/>
        public string X509Certificate
        {
            get { return this.x509CertificateField; }
            set { this.x509CertificateField = value; }
        }
    }
}
