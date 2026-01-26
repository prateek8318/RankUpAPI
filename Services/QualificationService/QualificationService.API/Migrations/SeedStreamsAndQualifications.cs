using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualificationService.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedStreamsAndQualifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, delete existing data (optional - uncomment if you want to clean existing data)
            // migrationBuilder.Sql("DELETE FROM Qualifications");
            // migrationBuilder.Sql("DELETE FROM Streams");

            // Insert Streams
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM Streams WHERE Name = 'Science')
                BEGIN
                    INSERT INTO Streams (Name, Description, IsActive, CreatedAt) VALUES
                    ('Science', 'Science stream with Physics, Chemistry, Biology/Mathematics', 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Streams WHERE Name = 'Commerce')
                BEGIN
                    INSERT INTO Streams (Name, Description, IsActive, CreatedAt) VALUES
                    ('Commerce', 'Commerce stream with Accountancy, Business Studies, Economics', 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Streams WHERE Name = 'Arts')
                BEGIN
                    INSERT INTO Streams (Name, Description, IsActive, CreatedAt) VALUES
                    ('Arts', 'Arts/Humanities stream with History, Geography, Political Science', 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Streams WHERE Name = 'General')
                BEGIN
                    INSERT INTO Streams (Name, Description, IsActive, CreatedAt) VALUES
                    ('General', 'General stream for exams that don''t require specific stream', 1, GETDATE());
                END
            ");

            // Insert Qualifications with Streams
            migrationBuilder.Sql(@"
                DECLARE @ScienceStreamId INT = (SELECT Id FROM Streams WHERE Name = 'Science');
                DECLARE @CommerceStreamId INT = (SELECT Id FROM Streams WHERE Name = 'Commerce');
                DECLARE @ArtsStreamId INT = (SELECT Id FROM Streams WHERE Name = 'Arts');
                DECLARE @GeneralStreamId INT = (SELECT Id FROM Streams WHERE Name = 'General');

                -- 10th Grade (No specific stream required)
                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = '10th Grade')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('10th Grade', 'Secondary School Certificate (SSC)', @GeneralStreamId, 1, GETDATE());
                END

                -- 12th Grade with different streams
                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = '12th Grade - Science')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('12th Grade - Science', 'Higher Secondary Certificate (HSC) - Science Stream', @ScienceStreamId, 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = '12th Grade - Commerce')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('12th Grade - Commerce', 'Higher Secondary Certificate (HSC) - Commerce Stream', @CommerceStreamId, 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = '12th Grade - Arts')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('12th Grade - Arts', 'Higher Secondary Certificate (HSC) - Arts Stream', @ArtsStreamId, 1, GETDATE());
                END

                -- Graduation
                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = 'Graduation - Science')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('Graduation - Science', 'Bachelor''s Degree in Science (B.Sc.)', @ScienceStreamId, 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = 'Graduation - Commerce')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('Graduation - Commerce', 'Bachelor''s Degree in Commerce (B.Com.)', @CommerceStreamId, 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = 'Graduation - Arts')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('Graduation - Arts', 'Bachelor''s Degree in Arts (B.A.)', @ArtsStreamId, 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = 'Graduation - General')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('Graduation - General', 'Any Bachelor''s Degree', @GeneralStreamId, 1, GETDATE());
                END

                -- Post Graduation
                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = 'Post Graduation - Science')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('Post Graduation - Science', 'Master''s Degree in Science (M.Sc.)', @ScienceStreamId, 1, GETDATE());
                END

                IF NOT EXISTS (SELECT * FROM Qualifications WHERE Name = 'Post Graduation - General')
                BEGIN
                    INSERT INTO Qualifications (Name, Description, StreamId, IsActive, CreatedAt) VALUES
                    ('Post Graduation - General', 'Any Master''s Degree', @GeneralStreamId, 1, GETDATE());
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seeded data
            migrationBuilder.Sql(@"
                DELETE FROM Qualifications WHERE Name IN (
                    '10th Grade',
                    '12th Grade - Science',
                    '12th Grade - Commerce',
                    '12th Grade - Arts',
                    'Graduation - Science',
                    'Graduation - Commerce',
                    'Graduation - Arts',
                    'Graduation - General',
                    'Post Graduation - Science',
                    'Post Graduation - General'
                );

                DELETE FROM Streams WHERE Name IN ('Science', 'Commerce', 'Arts', 'General');
            ");
        }
    }
}
