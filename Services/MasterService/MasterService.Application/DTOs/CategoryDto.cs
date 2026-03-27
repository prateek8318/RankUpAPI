using System.ComponentModel.DataAnnotations;



namespace MasterService.Application.DTOs

{

    // Base classes for consistency

    public abstract class BaseDto

    {

        public int Id { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }



    public abstract class BaseCreateDto

    {

        // Optional Name field - only required for non-category DTOs

        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]

        public string? Name { get; set; }

    }



    public abstract class BaseUpdateDto : BaseCreateDto

    {

        public int Id { get; set; }

    }



    // Standardized response wrapper

    public class ApiResponse<T>

    {

        public bool Success { get; set; }

        public T? Data { get; set; }

        public string? Language { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Error { get; set; }

        public int? TotalCount { get; set; }

    }



    // Category DTOs with inheritance

    public class CategoryDto : BaseDto

    {

        // Localized name based on requested language (Hi select karne par Hindi, warna English)

        public string Name { get; set; } = string.Empty;



        // Stored English name from DB

        public string NameEn { get; set; } = string.Empty;



        // Stored Hindi name from DB (optional)

        public string? NameHi { get; set; }



        public string Key { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty; // category, qualification, exam_category, stream

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        

        // Status field instead of IsActive

        public string Status { get; set; } = "active"; // active, inactive, draft, archived

        

        // Keep IsActive for backward compatibility

        public bool IsActive => Status == "active";

    }



    public class CreateCategoryDto : BaseCreateDto

    {

        // English name (required)

        [Required(ErrorMessage = "NameEn is required")]

        [StringLength(100, ErrorMessage = "NameEn cannot exceed 100 characters")]

        public string NameEn { get; set; } = string.Empty;



        // Hindi name (optional)

        [StringLength(100, ErrorMessage = "NameHi cannot exceed 100 characters")]

        public string? NameHi { get; set; }



        [Required(ErrorMessage = "Key is required")]

        [StringLength(50, ErrorMessage = "Key cannot exceed 50 characters")]

        public string Key { get; set; } = string.Empty;



        [Required(ErrorMessage = "Type is required")]

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]

        public string Type { get; set; } = string.Empty;



        // Status field instead of IsActive

        public string Status { get; set; } = "active"; // active, inactive, draft, archived

        

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

    }



    public class UpdateCategoryDto : BaseUpdateDto

    {

        // English name (required)

        [Required(ErrorMessage = "NameEn is required")]

        [StringLength(100, ErrorMessage = "NameEn cannot exceed 100 characters")]

        public string NameEn { get; set; } = string.Empty;



        // Hindi name (optional)

        [StringLength(100, ErrorMessage = "NameHi cannot exceed 100 characters")]

        public string? NameHi { get; set; }



        [Required(ErrorMessage = "Key is required")]

        [StringLength(50, ErrorMessage = "Key cannot exceed 50 characters")]

        public string Key { get; set; } = string.Empty;



        [Required(ErrorMessage = "Type is required")]

        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]

        public string Type { get; set; } = string.Empty;



        // Status field instead of IsActive

        public string Status { get; set; } = "active"; // active, inactive, draft, archived

        

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

    }

}

