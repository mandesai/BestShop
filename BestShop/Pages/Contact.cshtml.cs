using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace BestShop.Pages
{
    public class ContactModel : PageModel
    {
        public void OnGet()
        {
        }

        [BindProperty, Display(Name = "First Name*")]
        [Required(ErrorMessage="The First Name is required")]
        public string FirstName { get; set; } = "";
        [BindProperty, Display(Name = "Last Name*"), Required(ErrorMessage ="The Last Name is required")]
        public string LastName { get; set; } = "";
        [BindProperty, Required(ErrorMessage = "The email is required")]
        [EmailAddress, Display(Name = "Email*")]
        public string Email { get; set; } = "";
        [BindProperty]
        public string? Phone { get; set; } = "";
        [BindProperty, Required(ErrorMessage="The Subject is required")]
        [Display(Name = "Subject*")]
        public string Subject { get; set; } = "";
        [BindProperty, Required(ErrorMessage="The message is required")]
        [MinLength(5, ErrorMessage = "The message should be at least 5 characters")]
        [MaxLength(1024, ErrorMessage = "The message should be less than 1024 characters")]
        [Display(Name = "Message*")]
        public string Message { get; set; } = "";

        public List<SelectListItem> SubjectList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Order Status", Text = "Order Status" },
            new SelectListItem { Value = "Refund Request", Text = "Refund Request" },
            new SelectListItem { Value = "Job Application", Text = "Job Application"  },
            new SelectListItem { Value = "Other", Text = "Other"  },
        };

        public string SuccessMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public void OnPost()
        {
            // check if any required field is empty
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill all required fields";
                return;
            }

            if (Phone == null) Phone = "";

            // Add this message to the database
            try
            {
                string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=bestshop;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO messages " +
                        "(firstname, lastname, email, phone, subject, message) VALUES " +
                        "(@firstname, @lastname, @email, @phone, @subject, @message);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", FirstName);
                        command.Parameters.AddWithValue("@lastname", LastName);
                        command.Parameters.AddWithValue("@email", Email);
                        command.Parameters.AddWithValue("@phone", Phone);
                        command.Parameters.AddWithValue("@subject", Subject);
                        command.Parameters.AddWithValue("@message", Message);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            SuccessMessage = "Your message has been received correctly.";

            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            Subject = "";
            Message = "";

            ModelState.Clear();
        }
    }
}
