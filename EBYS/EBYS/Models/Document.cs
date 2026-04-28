using System.ComponentModel.DataAnnotations;

namespace EBYS.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        // ===== ÜST VERİ =====
        public string BelgeTuru { get; set; } = "Dilekçe";
        public DateTime? BelgeTarihi { get; set; } = DateTime.Now;
        public bool BilaTarih { get; set; }

        public string BelgeKategorisi { get; set; } = "Kurum İçi Yazışma";
        public string Konu { get; set; } = string.Empty;
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;

        public string UreticiBelgesi { get; set; } = string.Empty;
        public string UretimYeri { get; set; } = string.Empty;
        public string Dil { get; set; } = "Türkçe";

        // ===== CHECKBOXES =====
        public bool TelifYasasi { get; set; }
        public bool BilgiEdinme { get; set; }
        public bool KisiselBilgi { get; set; }
        public bool ZorunluHal { get; set; }
        public bool OlaganustuDurum { get; set; }
        public bool BirimHiyerarsi { get; set; }

        public string GizlilikDerecesi { get; set; } = "Hizmete Özel";

        // ===== CONTENT =====
        public string EditorContent { get; set; } = string.Empty;

        // ===== SIGN =====
        // ✅ safer: can be null until the user selects signer
        public string? SignerUserId { get; set; }

        public string? ApprovedByName { get; set; }
        public string? ApprovedByTitle { get; set; }

        // ===== META =====
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }

        // Draft | Sent | Approved | Rejected | Archived
        public string Durum { get; set; } = "Draft";

        // ===== SOFT DELETE =====
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // ===== EBYS =====
        public string Sayi { get; set; } = string.Empty;
    }
}
