# Test Series API - Postman Collection Endpoints

## Base URL
```
http://localhost:5234/api/admin
```
या आपका production URL

## Authentication
सभी endpoints के लिए Header में:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## 1. SUBJECT ENDPOINTS

### 1.1 Get All Subjects
```
GET /api/admin/subject
GET /api/admin/subject?examId=1
```

### 1.2 Get Subject By ID
```
GET /api/admin/subject/{id}
```
Example: `GET /api/admin/subject/1`

### 1.3 Create Subject
```
POST /api/admin/subject
Content-Type: application/json

Body:
{
  "name": "Mathematics",
  "description": "Mathematics subject",
  "examId": 1
}
```

### 1.4 Update Subject
```
PUT /api/admin/subject/{id}
Content-Type: application/json

Body:
{
  "id": 1,
  "name": "Mathematics Updated",
  "description": "Updated description",
  "isActive": true
}
```

### 1.5 Delete Subject
```
DELETE /api/admin/subject/{id}
```

### 1.6 Toggle Subject Status (Enable/Disable)
```
PATCH /api/admin/subject/{id}/status
Content-Type: application/json

Body:
true
```
या
```
false
```

---

## 2. CHAPTER ENDPOINTS

### 2.1 Get All Chapters
```
GET /api/admin/chapter
GET /api/admin/chapter?subjectId=1
```

### 2.2 Get Chapter By ID
```
GET /api/admin/chapter/{id}
```
Example: `GET /api/admin/chapter/1`

### 2.3 Create Chapter
```
POST /api/admin/chapter
Content-Type: application/json

Body:
{
  "name": "Algebra",
  "description": "Algebra chapter",
  "subjectId": 1
}
```

### 2.4 Update Chapter
```
PUT /api/admin/chapter/{id}
Content-Type: application/json

Body:
{
  "id": 1,
  "name": "Algebra Updated",
  "description": "Updated description",
  "isActive": true
}
```

### 2.5 Delete Chapter
```
DELETE /api/admin/chapter/{id}
```

### 2.6 Toggle Chapter Status (Enable/Disable)
```
PATCH /api/admin/chapter/{id}/status
Content-Type: application/json

Body:
true
```
या
```
false
```

---

## 3. TEST SERIES ENDPOINTS

### 3.1 Get All Test Series
```
GET /api/admin/testseries
GET /api/admin/testseries?examId=1
```

### 3.2 Get Test Series By ID
```
GET /api/admin/testseries/{id}
```
Example: `GET /api/admin/testseries/1`

### 3.3 Create Test Series
```
POST /api/admin/testseries
Content-Type: application/json

Body:
{
  "name": "Railway Group D Test Series 01",
  "description": "First test series",
  "examId": 1,
  "durationInMinutes": 60,
  "instructionsEnglish": "Read all instructions carefully",
  "instructionsHindi": "सभी निर्देश ध्यान से पढ़ें",
  "displayOrder": 1,
  "isLocked": false
}
```

**Note:** 
- Test Series is directly linked to Exam only (no Subject/Chapter needed)
- `totalMarks` is automatically calculated from number of questions (1 question = 1 mark)
- Maximum 100 questions allowed per test series
- `durationInMinutes` is required (1-600 minutes)

### 3.4 Update Test Series
```
PUT /api/admin/testseries/{id}
Content-Type: application/json

Body:
{
  "id": 1,
  "name": "Railway Group D Test Series 01 Updated",
  "description": "Updated description",
  "durationInMinutes": 90,
  "instructionsEnglish": "Updated instructions",
  "instructionsHindi": "अपडेटेड निर्देश",
  "displayOrder": 1,
  "isLocked": false,
  "isActive": true
}
```

**Note:** 
- `totalMarks` is automatically calculated from number of questions
- No need to specify `totalMarks` or `passingMarks` manually

### 3.5 Delete Test Series
```
DELETE /api/admin/testseries/{id}
```

### 3.6 Toggle Test Series Status (Enable/Disable)
```
PATCH /api/admin/testseries/{id}/status
Content-Type: application/json

Body:
true
```
या
```
false
```

### 3.7 Add Questions to Test Series
```
POST /api/admin/testseries/add-questions
Content-Type: application/json

Body:
{
  "testSeriesId": 1,
  "questionIds": [1, 2, 3, 4, 5]
}
```

### 3.8 Remove Question from Test Series
```
DELETE /api/admin/testseries/{testSeriesId}/questions/{questionId}
```
Example: `DELETE /api/admin/testseries/1/questions/5`

---

## 4. QUESTION ENDPOINTS

### 4.1 Get All Questions
```
GET /api/admin/question
GET /api/admin/question?chapterId=1
GET /api/admin/question?subjectId=1
GET /api/admin/question?examId=1
```

### 4.2 Get Question By ID
```
GET /api/admin/question/{id}
```
Example: `GET /api/admin/question/1`

### 4.3 Create Question
```
POST /api/admin/question
Content-Type: application/json

Body:
{
  "questionTextEnglish": "Which of the following is the best conductor of electricity?",
  "questionTextHindi": "निम्नलिखित में से कौन सा बिजली का सबसे अच्छा कंडक्टर है?",
  "type": 1,
  "optionAEnglish": "Cold Water",
  "optionBEnglish": "Saline Water",
  "optionCEnglish": "Distilled Water",
  "optionDEnglish": "Warm Water",
  "optionAHindi": "ठंडा पानी",
  "optionBHindi": "नमकीन पानी",
  "optionCHindi": "आसुत जल",
  "optionDHindi": "गर्म पानी",
  "correctAnswer": "B",
  "explanationEnglish": "Saline water contains ions which conduct electricity.",
  "explanationHindi": "नमकीन पानी में आयन होते हैं जो बिजली का संचालन करते हैं।",
  "difficulty": 2,
  "chapterId": 1,
  "marks": 1,
  "negativeMarks": 0.25,
  "estimatedTimeInSeconds": 120,
  "isMcq": true
}
```

**Question Type Values:**
- `1` = Text
- `2` = Image
- `3` = Video

**Difficulty Values:**
- `1` = Easy
- `2` = Medium
- `3` = Hard

### 4.4 Update Question
```
PUT /api/admin/question/{id}
Content-Type: application/json

Body:
{
  "id": 1,
  "questionTextEnglish": "Updated question text",
  "questionTextHindi": "अपडेटेड प्रश्न",
  "type": 1,
  "optionAEnglish": "Option A",
  "optionBEnglish": "Option B",
  "optionCEnglish": "Option C",
  "optionDEnglish": "Option D",
  "optionAHindi": "",
  "optionBHindi": "",
  "optionCHindi": "",
  "optionDHindi": "",
  "correctAnswer": "A",
  "explanationEnglish": "Explanation",
  "explanationHindi": "",
  "difficulty": 1,
  "chapterId": 1,
  "marks": 1,
  "negativeMarks": 0,
  "estimatedTimeInSeconds": 120,
  "isMcq": true,
  "isActive": true
}
```

### 4.5 Delete Question
```
DELETE /api/admin/question/{id}
```

### 4.6 Toggle Question Status (Enable/Disable)
```
PATCH /api/admin/question/{id}/status
Content-Type: application/json

Body:
true
```
या
```
false
```

### 4.7 Bulk Upload Questions (CSV)
```
POST /api/admin/question/bulk-upload
Content-Type: multipart/form-data

Body (form-data):
Key: file
Type: File
Value: [Select your CSV file]
```

**Response Example:**
```json
{
  "successCount": 10,
  "errorCount": 2,
  "errors": [
    "Row 5: Chapter with ID 10 not found",
    "Row 8: Correct answer must be A, B, C, or D"
  ],
  "questions": []
}
```

---

## 5. EXISTING EXAM ENDPOINTS (Reference)

### 5.1 Get All Exams
```
GET /api/exams
```

### 5.2 Get Exam By ID
```
GET /api/exams/{id}
```

### 5.3 Create Exam
```
POST /api/exams
Content-Type: application/json

Body:
{
  "name": "Railway Group D",
  "description": "Railway Group D exam",
  "durationInMinutes": 60,
  "totalMarks": 100,
  "passingMarks": 35,
  "qualificationIds": [1, 2]
}
```

---

## Complete Postman Collection JSON

नीचे complete Postman collection JSON है जिसे आप directly import कर सकते हैं:

```json
{
  "info": {
    "name": "Test Series API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Subject",
      "item": [
        {
          "name": "Get All Subjects",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/subject",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "subject"]
            }
          }
        },
        {
          "name": "Get Subject By ID",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/subject/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "subject", "1"]
            }
          }
        },
        {
          "name": "Create Subject",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"name\": \"Mathematics\",\n  \"description\": \"Mathematics subject\",\n  \"examId\": 1\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/subject",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "subject"]
            }
          }
        },
        {
          "name": "Update Subject",
          "request": {
            "method": "PUT",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"id\": 1,\n  \"name\": \"Mathematics Updated\",\n  \"description\": \"Updated description\",\n  \"isActive\": true\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/subject/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "subject", "1"]
            }
          }
        },
        {
          "name": "Delete Subject",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/subject/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "subject", "1"]
            }
          }
        },
        {
          "name": "Toggle Subject Status",
          "request": {
            "method": "PATCH",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "true"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/subject/1/status",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "subject", "1", "status"]
            }
          }
        }
      ]
    },
    {
      "name": "Chapter",
      "item": [
        {
          "name": "Get All Chapters",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/chapter",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "chapter"]
            }
          }
        },
        {
          "name": "Get Chapter By ID",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/chapter/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "chapter", "1"]
            }
          }
        },
        {
          "name": "Create Chapter",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"name\": \"Algebra\",\n  \"description\": \"Algebra chapter\",\n  \"subjectId\": 1\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/chapter",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "chapter"]
            }
          }
        },
        {
          "name": "Update Chapter",
          "request": {
            "method": "PUT",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"id\": 1,\n  \"name\": \"Algebra Updated\",\n  \"description\": \"Updated description\",\n  \"isActive\": true\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/chapter/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "chapter", "1"]
            }
          }
        },
        {
          "name": "Delete Chapter",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/chapter/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "chapter", "1"]
            }
          }
        },
        {
          "name": "Toggle Chapter Status",
          "request": {
            "method": "PATCH",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "true"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/chapter/1/status",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "chapter", "1", "status"]
            }
          }
        }
      ]
    },
    {
      "name": "Test Series",
      "item": [
        {
          "name": "Get All Test Series",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries"]
            }
          }
        },
        {
          "name": "Get Test Series By ID",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries", "1"]
            }
          }
        },
        {
          "name": "Create Test Series",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"name\": \"Railway Group D Test Series 01\",\n  \"description\": \"First test series\",\n  \"examId\": 1,\n  \"durationInMinutes\": 60,\n  \"instructionsEnglish\": \"Read all instructions carefully\",\n  \"instructionsHindi\": \"सभी निर्देश ध्यान से पढ़ें\",\n  \"displayOrder\": 1,\n  \"isLocked\": false\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries"]
            }
          }
        },
        {
          "name": "Update Test Series",
          "request": {
            "method": "PUT",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
             "body": {
               "mode": "raw",
               "raw": "{\n  \"id\": 1,\n  \"name\": \"Railway Group D Test Series 01 Updated\",\n  \"description\": \"Updated description\",\n  \"durationInMinutes\": 90,\n  \"instructionsEnglish\": \"Updated instructions\",\n  \"instructionsHindi\": \"अपडेटेड निर्देश\",\n  \"displayOrder\": 1,\n  \"isLocked\": false,\n  \"isActive\": true\n}"
             },
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries", "1"]
            }
          }
        },
        {
          "name": "Delete Test Series",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries", "1"]
            }
          }
        },
        {
          "name": "Toggle Test Series Status",
          "request": {
            "method": "PATCH",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "true"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries/1/status",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries", "1", "status"]
            }
          }
        },
        {
          "name": "Add Questions to Test Series",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"testSeriesId\": 1,\n  \"questionIds\": [1, 2, 3, 4, 5]\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries/add-questions",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries", "add-questions"]
            }
          }
        },
        {
          "name": "Remove Question from Test Series",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/testseries/1/questions/5",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "testseries", "1", "questions", "5"]
            }
          }
        }
      ]
    },
    {
      "name": "Question",
      "item": [
        {
          "name": "Get All Questions",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/question",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question"]
            }
          }
        },
        {
          "name": "Get Question By ID",
          "request": {
            "method": "GET",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/question/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question", "1"]
            }
          }
        },
        {
          "name": "Create Question",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"questionTextEnglish\": \"Which of the following is the best conductor of electricity?\",\n  \"questionTextHindi\": \"निम्नलिखित में से कौन सा बिजली का सबसे अच्छा कंडक्टर है?\",\n  \"type\": 1,\n  \"optionAEnglish\": \"Cold Water\",\n  \"optionBEnglish\": \"Saline Water\",\n  \"optionCEnglish\": \"Distilled Water\",\n  \"optionDEnglish\": \"Warm Water\",\n  \"optionAHindi\": \"ठंडा पानी\",\n  \"optionBHindi\": \"नमकीन पानी\",\n  \"optionCHindi\": \"आसुत जल\",\n  \"optionDHindi\": \"गर्म पानी\",\n  \"correctAnswer\": \"B\",\n  \"explanationEnglish\": \"Saline water contains ions which conduct electricity.\",\n  \"explanationHindi\": \"नमकीन पानी में आयन होते हैं जो बिजली का संचालन करते हैं।\",\n  \"difficulty\": 2,\n  \"chapterId\": 1,\n  \"marks\": 1,\n  \"negativeMarks\": 0.25,\n  \"estimatedTimeInSeconds\": 120,\n  \"isMcq\": true\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/question",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question"]
            }
          }
        },
        {
          "name": "Update Question",
          "request": {
            "method": "PUT",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"id\": 1,\n  \"questionTextEnglish\": \"Updated question text\",\n  \"questionTextHindi\": \"अपडेटेड प्रश्न\",\n  \"type\": 1,\n  \"optionAEnglish\": \"Option A\",\n  \"optionBEnglish\": \"Option B\",\n  \"optionCEnglish\": \"Option C\",\n  \"optionDEnglish\": \"Option D\",\n  \"optionAHindi\": \"\",\n  \"optionBHindi\": \"\",\n  \"optionCHindi\": \"\",\n  \"optionDHindi\": \"\",\n  \"correctAnswer\": \"A\",\n  \"explanationEnglish\": \"Explanation\",\n  \"explanationHindi\": \"\",\n  \"difficulty\": 1,\n  \"chapterId\": 1,\n  \"marks\": 1,\n  \"negativeMarks\": 0,\n  \"estimatedTimeInSeconds\": 120,\n  \"isMcq\": true,\n  \"isActive\": true\n}"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/question/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question", "1"]
            }
          }
        },
        {
          "name": "Delete Question",
          "request": {
            "method": "DELETE",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "url": {
              "raw": "{{baseUrl}}/api/admin/question/1",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question", "1"]
            }
          }
        },
        {
          "name": "Toggle Question Status",
          "request": {
            "method": "PATCH",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              },
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "true"
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/question/1/status",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question", "1", "status"]
            }
          }
        },
        {
          "name": "Bulk Upload Questions",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "body": {
              "mode": "formdata",
              "formdata": [
                {
                  "key": "file",
                  "type": "file",
                  "src": []
                }
              ]
            },
            "url": {
              "raw": "{{baseUrl}}/api/admin/question/bulk-upload",
              "host": ["{{baseUrl}}"],
              "path": ["api", "admin", "question", "bulk-upload"]
            }
          }
        }
      ]
    }
  ],
  "variable": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5000",
      "type": "string"
    },
    {
      "key": "token",
      "value": "",
      "type": "string"
    }
  ]
}
```

---

## Postman Variables Setup

Postman में Environment variables set करें:

1. **baseUrl**: `http://localhost:5000` (या आपका server URL)
2. **token**: आपका JWT token (Admin login से मिलेगा)

---

## Quick Reference Table

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/subject` | Get all subjects |
| GET | `/api/admin/subject/{id}` | Get subject by ID |
| POST | `/api/admin/subject` | Create subject |
| PUT | `/api/admin/subject/{id}` | Update subject |
| DELETE | `/api/admin/subject/{id}` | Delete subject |
| PATCH | `/api/admin/subject/{id}/status` | Toggle subject status |
| GET | `/api/admin/chapter` | Get all chapters |
| GET | `/api/admin/chapter/{id}` | Get chapter by ID |
| POST | `/api/admin/chapter` | Create chapter |
| PUT | `/api/admin/chapter/{id}` | Update chapter |
| DELETE | `/api/admin/chapter/{id}` | Delete chapter |
| PATCH | `/api/admin/chapter/{id}/status` | Toggle chapter status |
| GET | `/api/admin/testseries` | Get all test series |
| GET | `/api/admin/testseries/{id}` | Get test series by ID |
| POST | `/api/admin/testseries` | Create test series |
| PUT | `/api/admin/testseries/{id}` | Update test series |
| DELETE | `/api/admin/testseries/{id}` | Delete test series |
| PATCH | `/api/admin/testseries/{id}/status` | Toggle test series status |
| POST | `/api/admin/testseries/add-questions` | Add questions to test series |
| DELETE | `/api/admin/testseries/{testSeriesId}/questions/{questionId}` | Remove question from test series |
| GET | `/api/admin/question` | Get all questions |
| GET | `/api/admin/question/{id}` | Get question by ID |
| POST | `/api/admin/question` | Create question |
| PUT | `/api/admin/question/{id}` | Update question |
| DELETE | `/api/admin/question/{id}` | Delete question |
| PATCH | `/api/admin/question/{id}/status` | Toggle question status |
| POST | `/api/admin/question/bulk-upload` | Bulk upload questions (CSV) |

---

## Notes:

1. सभी endpoints के लिए `Authorization: Bearer {token}` header required है
2. `{id}`, `{testSeriesId}`, `{questionId}` को actual values से replace करें
3. Query parameters optional हैं (जैसे `?examId=1`)
4. Bulk upload के लिए `multipart/form-data` use करें और file select करें
