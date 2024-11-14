namespace ExcelDataImport.Models
{
    public class ExpaqClaim
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public DateTime? DateFiled { get; set; }
        public DateTime? DateReceived { get; set; }
        public string CourtStation { get; set; }
        public string Rank { get; set; }
        public string CaseNo { get; set; }
        public string Year { get; set; }
        public string Plaintiff { get; set; }
        public string Defendant { get; set; }
        public string ThirdPartyAdvocate { get; set; }
        public string InjuryType { get; set; }
        public DateTime? LossDate { get; set; }
        public string InsuredMV { get; set; }
        public string ClaimNo { get; set; }
        public string Region { get; set; }
        public string OurAdvocate { get; set; }

    }
}
