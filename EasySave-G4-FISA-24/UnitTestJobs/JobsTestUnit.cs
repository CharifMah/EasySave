using Models.Backup;
namespace UnitTestJobs
{
    public class JobsTestUnit
    {
        [Fact]
        public void CreateJob()
        {
            CJobManager lJobManager = new CJobManager();
            CJob lJob = new CJob("Job1", "", "", ETypeBackup.COMPLET);
            lJobManager.CreateBackupJob(lJob);
            Assert.Equal(lJobManager.Jobs.Count, 1);
            lJobManager.CreateBackupJob(lJob);
            Assert.Equal(lJobManager.Jobs.Count, 1);
            CJob lJob2 = new CJob("Job2", "", "", ETypeBackup.COMPLET);
            lJobManager.CreateBackupJob(lJob2);
            Assert.Equal(lJobManager.Jobs.Count, 2);
        }
        [Fact]
        public void SaveLoadJobManager()
        {
            CJobManager lJobManager = new CJobManager();
            CJob lJob = new CJob("Job1", "", "", ETypeBackup.COMPLET);
            lJobManager.CreateBackupJob(lJob);
            Assert.Equal(lJobManager.Jobs.Count, 1);
            lJobManager.SaveJobs();
            lJobManager = Models.Settings.Instance.LoadJobsFile();
            Assert.Equal(lJobManager.Jobs.First(), lJob);
            CJob lJob1 = new CJob("Job11", "", "", ETypeBackup.COMPLET);
            lJobManager.CreateBackupJob(lJob1);
            lJobManager = Models.Settings.Instance.LoadJobsFile();
            Assert.Equal(lJobManager.Jobs.First(), lJob);
            Assert.Equal(lJobManager.Jobs.Count, 1);
        }

        [Fact]
        public void SaveLoadJobs()
        {
            CJobManager lJobManager = new CJobManager();
            CJob lJob = new CJob("Job1", "", "", ETypeBackup.COMPLET);
            lJobManager.CreateBackupJob(lJob);
            lJobManager.SaveJobs();

            CJobManager lJobManager2 = Models.Settings.Instance.LoadJobsFile();

            Assert.True(lJobManager2.Jobs.Any());
        }
    }
}