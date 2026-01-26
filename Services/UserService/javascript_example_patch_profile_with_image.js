// JavaScript fetch example for patch profile with image
const formData = new FormData();
formData.append('FullName', 'John Doe');
formData.append('Email', 'john@example.com');
formData.append('Gender', 'Male');
formData.append('Dob', '1995-01-15');
formData.append('StateId', '1');
formData.append('LanguageId', '1');
formData.append('QualificationId', '1');
formData.append('ExamId', '1');
formData.append('IsInterestedInInternationalExam', 'true'); // Note: string value
formData.append('ProfilePhoto', fileInput.files[0]); // File from input

fetch('http://localhost:5002/api/users/profile-with-image', {
  method: 'PATCH',
  headers: {
    'Authorization': 'Bearer YOUR_JWT_TOKEN'
    // Don't set Content-Type - browser does it automatically for FormData
  },
  body: formData
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
