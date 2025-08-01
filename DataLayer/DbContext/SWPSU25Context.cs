using DataLayer.Entities;
using DataLayer.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DbContext
{
    public class SWPSU25Context : Microsoft.EntityFrameworkCore.DbContext
    {
        public SWPSU25Context() { }
        public SWPSU25Context(DbContextOptions<SWPSU25Context> options) : base(options)
        { }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ARVProtocol> ARVProtocols { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<LabResult> LabResults { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientTreatmentProtocol> PatientTreatmentProtocols { get; set; }
        public DbSet<TreatmentStage> TreatmentStages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<LabPicture> LabPictures { get; set; } // Thêm DbSet cho LabPicture
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = "Server=localhost;Database=SWPSU25;Trusted_Connection=True;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(conn);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // tạo sẵn admin trong database
            var  adminUserId = Guid.Parse("7f85377c-d97f-4219-b76c-2ae926013d79"); // Tạo ID duy nhất cho admin user

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId, // Đảm bảo khớp với tên thuộc tính Id của bạn
                    Username = "admin",
                    Password = "admin",
                    Email = "admin@yourdomain.com",
                    PhoneNumber = "0123456789",
                    Role = UserRole.Admin, // Đảm bảo UserRole.Admin tồn tại trong enum của bạn
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), // Dùng DateTime tĩnh
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)  // Dùng DateTime tĩnh
                }
            );

            // --- User Entity ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique(); // Username phải là duy nhất
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Email phải là duy nhất
            // Enum Conversion cho UserRole
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>(); // Lưu UserRole dưới dạng string

            // --- Patient Entity ---
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId); // Mối quan hệ 1-1 giữa User và Patient
            // Enum Conversion cho Gender
            modelBuilder.Entity<Patient>()
                .Property(p => p.Gender)
                .HasConversion<string>(); // Lưu Gender dưới dạng string

            // --- Doctor Entity ---
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId); // Mối quan hệ 1-1 giữa User và Doctor

            // --- Blog Entity ---
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany(c => c.Blog)
                .HasForeignKey(b => b.AuthorID)
                .IsRequired(false) // AuthorID có thể null
                .OnDelete(DeleteBehavior.SetNull); // Khi User bị xóa, AuthorID trong Blog sẽ thành null
            // Enum Conversion cho BlogTag
            modelBuilder.Entity<Blog>()
                .Property(b => b.Tags)
                .HasConversion<string>(); // Lưu BlogTag dưới dạng string

            // --- Appointment Entity ---
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa Patient khi xóa Appointment

            // Enum Conversion cho AppointmentType và AppointmentStatus
            modelBuilder.Entity<Appointment>()
                .Property(a => a.AppointmentType)
                .HasConversion<string>();
            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            // --- DoctorSchedule Entity ---
            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSchedules)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // DoctorSchedule có thể có 1 Appointment (hoặc không)
            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.Appointment)
                .WithMany() // Appointment không có ICollection<DoctorSchedule>
                .HasForeignKey(ds => ds.AppointmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // Khi Appointment bị xóa, AppointmentId trong DoctorSchedule thành null

            // --- MedicalRecord Entity ---
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- ARVProtocol Entity ---
            // Đã có ICollection<PatientTreatmentProtocol> trong ARVProtocol
            // Cấu hình được xử lý từ phía PatientTreatmentProtocol

            // --- PatientTreatmentProtocol Entity ---
            modelBuilder.Entity<PatientTreatmentProtocol>()
                .HasOne(ptp => ptp.Patient)
                .WithMany(p => p.PatientTreatmentProtocols)
                .HasForeignKey(ptp => ptp.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientTreatmentProtocol>()
                .HasOne(ptp => ptp.Doctor)
                .WithMany(d => d.PatientTreatmentProtocols)
                .HasForeignKey(ptp => ptp.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientTreatmentProtocol>()
                .HasOne(ptp => ptp.ARVProtocol)
                .WithMany(ap => ap.PatientTreatmentProtocols)
                .HasForeignKey(ptp => ptp.ARVProtocolId)
                .IsRequired(false) // ProtocolId có thể null
                .OnDelete(DeleteBehavior.SetNull); // Khi ARVProtocol bị xóa, ProtocolId trong PTP thành null

            // PatientTreatmentProtocol có thể có 1 Appointment (hoặc không)
            modelBuilder.Entity<PatientTreatmentProtocol>()
                .HasOne(ptp => ptp.Appointment)
                .WithMany() // Appointment không có ICollection<PatientTreatmentProtocol>
                .HasForeignKey(ptp => ptp.AppointmentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Enum Conversion cho PatientTreatmentStatus
            modelBuilder.Entity<PatientTreatmentProtocol>()
                .Property(ptp => ptp.Status)
                .HasConversion<string>();

            // --- TreatmentStage Entity ---

            // Mối quan hệ 1-N: PatientTreatmentProtocol có nhiều TreatmentStage
            modelBuilder.Entity<TreatmentStage>()
                .HasOne(ts => ts.PatientTreatmentProtocol)
                .WithMany(ptp => ptp.TreatmentStages)
                .HasForeignKey(ts => ts.PatientTreatmentProtocolId)
                .IsRequired() // Mặc định là bắt buộc, nhưng thêm để rõ ràng
                .OnDelete(DeleteBehavior.Restrict); // Không xóa Protocol khi xóa Stage
            // --- LabResult Entity ---
            modelBuilder.Entity<LabResult>()
                .HasOne(lr => lr.Patient)
                .WithMany(p => p.LabResults)
                .HasForeignKey(lr => lr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mối quan hệ 1-N: TreatmentStage có nhiều LabResult
            modelBuilder.Entity<LabResult>()
                .HasOne(lr => lr.TreatmentStage)
                .WithMany(ts => ts.LabResults)
                .HasForeignKey(lr => lr.TreatmentStageId)
                .IsRequired(false) // LabResult có thể không gắn với Stage cụ thể (nếu cần)
                .OnDelete(DeleteBehavior.SetNull); // Khi TreatmentStage bị xóa, LabResult.TreatmentStageId thành null
            modelBuilder.Entity<LabResult>()
            .HasMany(lr => lr.LabPictures) // LabResult có nhiều LabPictures
            .WithOne(lp => lp.LabResult)  // LabPicture thuộc về một LabResult
            .HasForeignKey(lp => lp.LabResultId) // Khóa ngoại là LabResultId trong LabPicture
            .OnDelete(DeleteBehavior.Cascade); // Khi LabResult bị xóa, LabPictures liên quan cũng bị xóa
            // --- PaymentTransaction Entity ---
            modelBuilder.Entity<PaymentTransaction>()
            .HasOne(pt => pt.Appointment) // Một PaymentTransaction có một Appointment
            .WithOne(a => a.PaymentTransaction) // Một Appointment có một PaymentTransaction
            .HasForeignKey<PaymentTransaction>(pt => pt.AppointmentId) // Khóa ngoại nằm trong PaymentTransaction
            .IsRequired(); // Bắt buộc phải có Appointment liên quan

            // Cấu hình khóa ngoại từ Appointment tới PaymentTransaction (cũng 1:1)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.PaymentTransaction)
                .WithOne() // Không cần WithOne(pt => pt.Appointment) ở đây nữa vì đã định nghĩa ở trên
                .HasForeignKey<Appointment>(a => a.PaymentTransactionId);

            // Đảm bảo không có hành động Cascade Delete nếu bạn không muốn
            // (ví dụ: khi xóa PaymentTransaction thì không xóa Appointment)
            // Nếu không có dòng này, mặc định có thể là Cascade Delete
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.Appointment)
                .WithOne(a => a.PaymentTransaction)
                .HasForeignKey<PaymentTransaction>(pt => pt.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc DeleteBehavior.NoAction

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Patient)
                .WithMany(p => p.Notifications)
                .HasForeignKey(n => n.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.TreatmentStage)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.TreatmentStageId)
                .OnDelete(DeleteBehavior.NoAction); // tránh xóa thông báo khi xoá TreatmentStage

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Appointment)
                .WithMany(a => a.Notifications)
                .HasForeignKey(n => n.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);
            // Giữ nguyên OnDelete(DeleteBehavior.Cascade) ở đây là hợp lý
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.MedicalRecord)
                .WithOne(mr => mr.Prescriptions)
                .HasForeignKey<Prescription>(p => p.MedicalRecordId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            // Mối quan hệ 1-nhiều: TreatmentStage và MedicalRecord
            // Đây là nơi bạn cần thay đổi!
            // SetNull là lựa chọn tốt nhất vì TreatmentStageId là nullable
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.TreatmentStage)
                .WithMany(ts => ts.MedicalRecords)
                .HasForeignKey(mr => mr.TreatmentStageId)
                .IsRequired(false) // Đảm bảo khóa ngoại có thể là NULL
                .OnDelete(DeleteBehavior.SetNull); // Sửa thành SetNull
            // Mối quan hệ 1-nhiều: Prescription và PrescriptionItem
            // Giữ nguyên OnDelete(DeleteBehavior.Cascade) là hợp lý
            modelBuilder.Entity<PrescriptionItem>()
                .HasOne(pi => pi.Prescription)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PrescriptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }    
}

