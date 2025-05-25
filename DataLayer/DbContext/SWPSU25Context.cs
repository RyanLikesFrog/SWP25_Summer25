using DataLayer.Entities;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = "Server=localhost;Database=default;Trusted_Connection=True;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(conn);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Cấu hình mối quan hệ và ánh xạ dữ liệu ---

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
                .HasForeignKey(ptp => ptp.ProtocolId)
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

            // Mối quan hệ 1-1: MedicalRecord có MỘT TreatmentStage (và ngược lại)
            // Khóa ngoại MedicalRecordID nằm trong TreatmentStage và sẽ là DUY NHẤT
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.TreatmentStage)
                .WithMany() // MedicalRecord có TreatmentStage, nhưng TreatmentStage không có thuộc tính điều hướng ngược về MedicalRecord
                .HasForeignKey(mr => mr.TreatmentStageId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

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

            base.OnModelCreating(modelBuilder);
        }
    }    
}

