namespace AlbertZhao.cn.Models
{
    public class Student
    {
        public long ID { get; set; }
        public int Age { get; set; }

        public string Name { get; set; }

        public string SubName { get; set; }
        public double Score { get; set; }

        public string GetStudentNameByID(int id)
        {
            return Name + "_" + id;
        }

        public double GetStuScoreByID(int id)
        {
            return Score;
        }
    }
}
