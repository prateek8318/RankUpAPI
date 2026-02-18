# CMS Content API - Sample Requests

## RankUp Terms and Conditions

### Create Terms and Conditions (POST)

**Endpoint:** `POST /api/cms`  
**Authorization:** Bearer Token (Admin role required)

**Request Body:**

```json
{
  "key": "terms_and_conditions",
  "translations": [
    {
      "languageCode": "en",
      "title": "Terms and Conditions",
      "content": "<h1>Terms and Conditions</h1><p>Welcome to RankUp. By accessing and using this platform, you agree to comply with and be bound by the following terms and conditions.</p><h2>1. Acceptance of Terms</h2><p>By using RankUp, you acknowledge that you have read, understood, and agree to be bound by these Terms and Conditions.</p><h2>2. User Responsibilities</h2><ul><li>You are responsible for maintaining the confidentiality of your account credentials.</li><li>You agree to provide accurate and complete information when creating your account.</li><li>You must not use the platform for any illegal or unauthorized purpose.</li></ul><h2>3. Intellectual Property</h2><p>All content, features, and functionality of RankUp are owned by us and are protected by international copyright, trademark, and other intellectual property laws.</p><h2>4. Limitation of Liability</h2><p>RankUp shall not be liable for any indirect, incidental, special, or consequential damages arising out of or in connection with your use of the platform.</p><h2>5. Changes to Terms</h2><p>We reserve the right to modify these terms at any time. Continued use of the platform after changes constitutes acceptance of the new terms.</p><p><strong>Last Updated:</strong> February 16, 2026</p>"
    },
    {
      "languageCode": "hi",
      "title": "नियम और शर्तें",
      "content": "<h1>नियम और शर्तें</h1><p>RankUp में आपका स्वागत है। इस प्लेटफॉर्म तक पहुंचने और उपयोग करने से, आप निम्नलिखित नियमों और शर्तों का पालन करने और उनसे बाध्य होने के लिए सहमत हैं।</p><h2>1. नियमों की स्वीकृति</h2><p>RankUp का उपयोग करके, आप स्वीकार करते हैं कि आपने इन नियमों और शर्तों को पढ़ा, समझा है और उनसे बाध्य होने के लिए सहमत हैं।</p><h2>2. उपयोगकर्ता जिम्मेदारियां</h2><ul><li>आप अपने खाते की साख की गोपनीयता बनाए रखने के लिए जिम्मेदार हैं।</li><li>आप अपना खाता बनाते समय सटीक और पूर्ण जानकारी प्रदान करने के लिए सहमत हैं।</li><li>आपको प्लेटफॉर्म का उपयोग किसी भी अवैध या अनधिकृत उद्देश्य के लिए नहीं करना चाहिए।</li></ul><h2>3. बौद्धिक संपदा</h2><p>RankUp की सभी सामग्री, सुविधाएं और कार्यक्षमता हमारी संपत्ति हैं और अंतर्राष्ट्रीय कॉपीराइट, ट्रेडमार्क और अन्य बौद्धिक संपदा कानूनों द्वारा संरक्षित हैं।</p><h2>4. दायित्व की सीमा</h2><p>RankUp प्लेटफॉर्म के आपके उपयोग से उत्पन्न या उससे जुड़े किसी भी अप्रत्यक्ष, आकस्मिक, विशेष या परिणामी नुकसान के लिए उत्तरदायी नहीं होगा।</p><h2>5. नियमों में परिवर्तन</h2><p>हम किसी भी समय इन नियमों को संशोधित करने का अधिकार सुरक्षित रखते हैं। परिवर्तनों के बाद प्लेटफॉर्म का निरंतर उपयोग नए नियमों की स्वीकृति का गठन करता है।</p><p><strong>अंतिम अपडेट:</strong> 16 फरवरी, 2026</p>"
    }
  ]
}
```

**cURL Example:**

```bash
curl -X POST "https://your-api-domain.com/api/cms" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "key": "terms_and_conditions",
    "translations": [
      {
        "languageCode": "en",
        "title": "Terms and Conditions",
        "content": "<h1>Terms and Conditions</h1><p>Welcome to RankUp...</p>"
      },
      {
        "languageCode": "hi",
        "title": "नियम और शर्तें",
        "content": "<h1>नियम और शर्तें</h1><p>RankUp में आपका स्वागत है...</p>"
      }
    ]
  }'
```

**Response (201 Created):**

```json
{
  "id": 1,
  "key": "terms_and_conditions",
  "title": "Terms and Conditions",
  "content": "<h1>Terms and Conditions</h1><p>Welcome to RankUp...</p>",
  "status": "Active",
  "createdAt": "2026-02-16T10:00:00Z",
  "updatedAt": null,
  "translations": [
    {
      "languageCode": "en",
      "title": "Terms and Conditions",
      "content": "<h1>Terms and Conditions</h1><p>Welcome to RankUp...</p>"
    },
    {
      "languageCode": "hi",
      "title": "नियम और शर्तें",
      "content": "<h1>नियम और शर्तें</h1><p>RankUp में आपका स्वागत है...</p>"
    }
  ]
}
```

---

## Get Terms and Conditions (User-facing)

**Endpoint:** `GET /api/cms/terms_and_conditions?language=en`  
**Authorization:** Not required (public)

**Response:**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "key": "terms_and_conditions",
    "title": "Terms and Conditions",
    "content": "<h1>Terms and Conditions</h1><p>Welcome to RankUp...</p>",
    "status": "Active",
    "createdAt": "2026-02-16T10:00:00Z",
    "updatedAt": null
  },
  "language": "en",
  "message": "CMS content fetched successfully"
}
```

---

## Notes

1. **HTML Content Support:** 
   - Both `title` and `content` fields support full HTML markup
   - Content field has no length limit (`nvarchar(max)`)
   - Title field supports up to 500 characters (including HTML tags)

2. **Required Fields:**
   - `key` must be one of the allowed keys (see `GET /api/cms/keys`)
   - At least one translation with `languageCode: "en"` is mandatory
   - Other languages (hi, ta, gu) are optional

3. **Allowed Keys:**
   - `terms_and_conditions`
   - `privacy_policy`
   - `about_us`
   - `faq`
   - `contact_us`

4. **Error Responses:**
   - **400 Bad Request:** Invalid key or missing required fields
   - **409 Conflict:** Key already exists (use PUT to update)
   - **401 Unauthorized:** Missing or invalid admin token
