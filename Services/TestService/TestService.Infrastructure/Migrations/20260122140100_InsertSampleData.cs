using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestService.Infrastructure.Migrations
{
    public partial class InsertSampleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExamMasters",
                columns: new[] { "Name", "Description", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { "UPSC Civil Services", "Union Public Service Commission Civil Services Examination", 1, true },
                    { "SSC CGL", "Staff Selection Commission Combined Graduate Level Examination", 2, true },
                    { "IBPS PO", "Institute of Banking Personnel Selection Probationary Officer", 3, true }
                });

            migrationBuilder.InsertData(
                table: "SubjectMasters",
                columns: new[] { "ExamId", "Name", "Description", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "History", "Indian History and World History", 1, true },
                    { 1, "Geography", "Indian and World Geography", 2, true },
                    { 1, "Polity", "Indian Constitution and Political System", 3, true },
                    { 1, "Economy", "Indian Economy and Economic Development", 4, true },
                    { 2, "Reasoning", "Logical Reasoning and Analytical Ability", 1, true },
                    { 2, "Quantitative Aptitude", "Mathematics and Numerical Ability", 2, true },
                    { 2, "English Language", "English Grammar and Comprehension", 3, true },
                    { 2, "General Awareness", "General Knowledge and Current Affairs", 4, true }
                });

            migrationBuilder.InsertData(
                table: "TestSeries",
                columns: new[] { "Name", "Description", "ExamId", "DurationInMinutes", "TotalMarks", "TotalQuestions", "PassingMarks", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { "UPSC Prelims Test Series 2024", "Comprehensive test series for UPSC Prelims examination", 1, 120, 200, 100, 66, 1, true },
                    { "UPSC Mains Test Series 2024", "Comprehensive test series for UPSC Mains examination", 1, 180, 250, 20, 125, 2, true },
                    { "SSC CGL Mock Test Series", "Complete mock test series for SSC CGL preparation", 2, 60, 200, 100, 100, 1, true }
                });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "ExamId", "PracticeModeId", "SeriesId", "SubjectId", "Year", "Title", "Description", "DurationInMinutes", "TotalQuestions", "TotalMarks", "PassingMarks", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    // Mock Tests
                    { 1, 3, null, null, null, "UPSC Prelims Full Mock Test 1", "Complete mock test for UPSC Prelims preparation", 120, 100, 200, 66, 1, true },
                    { 1, 3, null, null, null, "UPSC Prelims Full Mock Test 2", "Complete mock test for UPSC Prelims preparation", 120, 100, 200, 66, 2, true },
                    
                    // Test Series Tests
                    { 1, 4, 1, null, null, "UPSC Prelims Test 1 - History & Culture", "First test in UPSC Prelims series focusing on History", 120, 25, 50, 33, 1, true },
                    { 1, 4, 1, null, null, "UPSC Prelims Test 2 - Geography", "Second test in UPSC Prelims series focusing on Geography", 120, 25, 50, 33, 2, true },
                    
                    // Deep Practice Tests
                    { 1, 5, null, 1, null, "Ancient History Deep Practice", "Focused practice on Ancient Indian History", 60, 20, 40, 24, 1, true },
                    { 1, 5, null, 2, null, "Physical Geography Deep Practice", "Focused practice on Physical Geography", 60, 20, 40, 24, 2, true },
                    
                    // Previous Year Tests
                    { 1, 6, null, null, 2023, "UPSC Prelims 2023 Previous Year Paper", "Previous year question paper from UPSC Prelims 2023", 120, 100, 200, 66, 1, true },
                    { 1, 6, null, null, 2022, "UPSC Prelims 2022 Previous Year Paper", "Previous year question paper from UPSC Prelims 2022", 120, 100, 200, 66, 2, true }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionText", "Explanation", "Difficulty", "Marks", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { "The Harappan civilization was mainly concentrated in which river valley?", "The Harappan civilization was primarily concentrated in the Indus River valley, with major cities like Mohenjo-Daro and Harappa located along its banks.", 1, 2, 1, true },
                    { "Which of the following is not a feature of the Indian Constitution?", "The Indian Constitution features a parliamentary system, fundamental rights, and directive principles, but does not have a rigid amendment procedure like the US Constitution.", 2, 2, 2, true },
                    { "What is the current GDP growth rate of India for 2023-24?", "According to recent estimates, India's GDP growth rate for 2023-24 is projected to be around 6.5-7%.", 2, 2, 3, true },
                    { "The Western Ghats are also known as:", "The Western Ghats are also known as the Sahyadri Hills, running parallel to the western coast of India.", 1, 2, 4, true }
                });

            migrationBuilder.InsertData(
                table: "TestQuestions",
                columns: new[] { "TestId", "QuestionId", "DisplayOrder", "Marks", "IsActive" },
                values: new object[,]
                {
                    // Mock Test 1 Questions
                    { 1, 1, 1, 2, true },
                    { 1, 2, 2, 2, true },
                    { 1, 3, 3, 2, true },
                    { 1, 4, 4, 2, true },
                    
                    // Test Series 1 Questions
                    { 3, 1, 1, 2, true },
                    { 3, 2, 2, 2, true },
                    
                    // Deep Practice 1 Questions
                    { 5, 1, 1, 2, true },
                    
                    // Previous Year 2023 Questions
                    { 7, 1, 1, 2, true },
                    { 7, 3, 2, 2, true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TestQuestions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Tests",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TestSeries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TestSeries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TestSeries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ExamMasters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExamMasters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExamMasters",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
