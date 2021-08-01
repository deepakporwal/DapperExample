using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using AutoMapper;
using Newtonsoft.Json;

namespace DapperExample
{
    class Program
    {
        public const string cnnStr = @"Data Source=DESKTOP-SAEKUJH\SQLEXPRESS;Initial Catalog=SampleDb;User ID=sa;Password=deepak";
        //string sql = "select * from student";
        public const string spName = "usp_demo";

        static void Main(string[] args)
        {
           

            //initialize the mapper
            var config = new MapperConfiguration(cgf =>
            {
                // Create map first parameter is Source and Second is destination
                cgf.CreateMap<ResultDATA, ResultDATADTO>().ReverseMap();
                
            });

            var mapper = new Mapper(config);

            using (var connection = new SqlConnection(cnnStr))
            {
                //var studentData = connection.Query<Student>(sql).ToList();

                var data = connection.QueryMultiple(spName, null, commandType: System.Data.CommandType.StoredProcedure);

                ResultDATA objData = new ResultDATA();

                objData.Students = data.Read<Student>().ToList();
                objData.StudentClasses = data.Read<udtClass>().ToList();

                Console.WriteLine(JsonConvert.SerializeObject(objData));

                var resDTO = mapper.Map<ResultDATA>(objData);
                Console.WriteLine("ResultDTO mapping");
                Console.WriteLine(JsonConvert.SerializeObject(resDTO));

                //Console.WriteLine(JsonConvert.SerializeObject(studentData));
                DemoFunctions obj = new DemoFunctions();
                Task<ResultDATADTO> finalData =  obj.GetData();
                Console.WriteLine(JsonConvert.SerializeObject(finalData));
            }

            Console.ReadLine();
        }

        public class Student
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }

        }

        public class StudentDTO
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
        }

        public class udtClass
        {
            public int ID { get; set; }
            public int Class { get; set; }
        }

        public class udtClassDTO
        {
            public int ID { get; set; }
            public int Class { get; set; }
        }
        public class ResultDATA
        {
            public List<Student> Students { get; set; }
            public List<udtClass> StudentClasses { get; set; }
        }

        public class ResultDATADTO
        {
            public List<StudentDTO> Students { get; set; }
            public List<udtClassDTO> StudentClasses { get; set; }
        }

        public class DemoFunctions
        {
            public async Task<ResultDATADTO> GetData()
            {
                using (var conn = new SqlConnection(cnnStr))
                {
                    var data = await conn.QueryMultipleAsync(spName, null, commandType: System.Data.CommandType.StoredProcedure);
                    //var count = data.Read<udtClass>().Count();
                    //var result = data.Read<udtClass>().ToList();

                    ResultDATADTO oData = new ResultDATADTO();
                    oData.Students = data.Read<StudentDTO>().ToList();
                    oData.StudentClasses = data.Read<udtClassDTO>().ToList();
                    Console.WriteLine(JsonConvert.SerializeObject(oData));
                    return oData;
                }
            }
        }
    
}
}
