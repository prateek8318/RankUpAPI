# Seed Data Instructions - Streams, Qualifications और Exams

## Overview
यह document बताता है कि कैसे database में streams, qualifications और exams का data add करें।

## Steps to Seed Data

### Step 1: QualificationService में Streams और Qualifications Seed करें

**Endpoint:**
```
POST /api/seed/qualifications?clearExisting=true
```

**Headers:**
```
Authorization: Bearer {Admin_Token}
Content-Type: application/json
```

**Query Parameters:**
- `clearExisting` (optional, default: false): अगर `true` है तो पुराना data delete हो जाएगा

**Example (Postman):**
1. Method: `POST`
2. URL: `http://localhost:5011/api/seed/qualifications?clearExisting=true`
3. Headers में Admin JWT token add करें
4. Send करें

**यह क्या करेगा:**
- 4 Streams create करेगा: Science, Commerce, Arts, General
- 10 Qualifications create करेगा:
  - 10th Grade (General stream)
  - 12th Grade - Science (Science stream)
  - 12th Grade - Commerce (Commerce stream)
  - 12th Grade - Arts (Arts stream)
  - Graduation - Science (Science stream)
  - Graduation - Commerce (Commerce stream)
  - Graduation - Arts (Arts stream)
  - Graduation - General (General stream)
  - Post Graduation - Science (Science stream)
  - Post Graduation - General (General stream)

### Step 2: ExamService में Exams Seed करें

**Important:** पहले QualificationService में data seed करना जरूरी है!

**Endpoint:**
```
POST /api/seed/exams?clearExisting=true
```

**Headers:**
```
Authorization: Bearer {Admin_Token}
Content-Type: application/json
```

**Query Parameters:**
- `clearExisting` (optional, default: false): अगर `true` है तो पुराना data delete हो जाएगा

**Example (Postman):**
1. Method: `POST`
2. URL: `http://localhost:5000/api/seed/exams?clearExisting=true`
3. Headers में Admin JWT token add करें
4. Send करें

**यह क्या करेगा:**

**National Exams (8 exams):**
1. **JEE Main** - 12th Science, Science stream
2. **JEE Advanced** - 12th Science, Science stream
3. **NEET** - 12th Science, Science stream
4. **UPSC Civil Services** - Graduation General, General stream
5. **SSC CGL** - Graduation General, General stream
6. **Banking PO** - Graduation General, General stream
7. **Railway NTPC** - Graduation General, General stream
8. **Teaching CTET** - Graduation General, General stream

**International Exams (7 exams):**
1. **SAT** - 12th Science, Science stream
2. **IELTS Academic** - 12th Science, Science stream
3. **TOEFL** - 12th Science, Science stream
4. **GRE** - Graduation General, General stream
5. **GMAT** - Graduation General, General stream
6. **PLAB** - Graduation General, General stream
7. **OET** - Graduation General, General stream

## Complete Workflow

### Option 1: Fresh Start (सभी पुराना data हटाना)
```bash
# Step 1: QualificationService में data seed करें
POST http://localhost:5011/api/seed/qualifications?clearExisting=true

# Step 2: ExamService में data seed करें
POST http://localhost:5000/api/seed/exams?clearExisting=true
```

### Option 2: Add Only (पुराना data रखना)
```bash
# Step 1: QualificationService में data seed करें
POST http://localhost:5011/api/seed/qualifications?clearExisting=false

# Step 2: ExamService में data seed करें
POST http://localhost:5000/api/seed/exams?clearExisting=false
```

## Verification

### Check Streams
```
GET http://localhost:5011/api/streams
```

### Check Qualifications
```
GET http://localhost:5011/api/qualifications
```

### Check Exams (National)
```
GET http://localhost:5000/api/exams?isInternational=false
```

### Check Exams (International)
```
GET http://localhost:5000/api/exams?isInternational=true
```

### Check Exams by Qualification and Stream
```
GET http://localhost:5000/api/exams?qualificationId=2&streamId=1&isInternational=false
```
(qualificationId और streamId अपने database के according change करें)

## Important Notes

1. **Order Matters:** पहले QualificationService में seed करें, फिर ExamService में
2. **QualificationService Running:** ExamService seed करते समय QualificationService running होना चाहिए
3. **Admin Token Required:** दोनों endpoints के लिए Admin role का JWT token चाहिए
4. **Database Connection:** दोनों services के databases properly configured होने चाहिए

## Troubleshooting

### Error: "Could not fetch qualifications from QualificationService"
- QualificationService running है या नहीं check करें
- `appsettings.json` में `QualificationService:BaseUrl` correct है या नहीं check करें

### Error: "Could not fetch streams from QualificationService"
- पहले QualificationService में seed endpoint run करें

### Exams नहीं दिख रहे
- Qualification और Stream IDs check करें
- ExamService में exams seed करने के बाद verify करें

## Postman Collection Example

```json
{
  "info": {
    "name": "Seed Data APIs"
  },
  "item": [
    {
      "name": "Seed Qualifications",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer YOUR_ADMIN_TOKEN"
          }
        ],
        "url": {
          "raw": "http://localhost:5011/api/seed/qualifications?clearExisting=true",
          "host": ["localhost"],
          "port": "5011",
          "path": ["api", "seed", "qualifications"],
          "query": [
            {
              "key": "clearExisting",
              "value": "true"
            }
          ]
        }
      }
    },
    {
      "name": "Seed Exams",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer YOUR_ADMIN_TOKEN"
          }
        ],
        "url": {
          "raw": "http://localhost:5000/api/seed/exams?clearExisting=true",
          "host": ["localhost"],
          "port": "5000",
          "path": ["api", "seed", "exams"],
          "query": [
            {
              "key": "clearExisting",
              "value": "true"
            }
          ]
        }
      }
    }
  ]
}
```

## Summary

1. ✅ QualificationService में streams और qualifications seed करें
2. ✅ ExamService में national और international exams seed करें
3. ✅ Data verify करें
4. ✅ APIs test करें

**सब कुछ ready है!** 🎉
