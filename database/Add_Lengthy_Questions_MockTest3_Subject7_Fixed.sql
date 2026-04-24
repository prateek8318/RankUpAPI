-- Add Lengthy Questions for Mock Test ID 3 with Subject ID 7 (Fixed Version)
-- =============================================

-- First check Subject ID 7 exists and get its details
SELECT Id, Name FROM Subjects WHERE Id = 7;

-- Create lengthy questions with proper translations
-- These will be comprehensive questions with detailed options

-- Question 1: Indian Constitution Basics
INSERT INTO Questions (
    ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD, 
    CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType, 
    SameExplanationForAllLanguages, Reference, Tags, CreatedBy, IsPublished, IsActive
)
VALUES (
    1, -- ModuleId
    1, -- ExamId (using a generic exam ID since Exams table is in different database)
    7, -- SubjectId
    NULL, -- TopicId
    'The Indian Constitution, adopted on 26th November 1949 and enforced on 26th January 1950, is the supreme law of India. It lays down the framework defining fundamental political principles, establishes the structure, procedures, powers, and duties of government institutions, and sets out fundamental rights, directive principles, and the duties of citizens. Which of the following statements correctly describes the nature of the Indian Constitution?',
    
    'The Indian Constitution is a rigid and federal constitution with a strong unitary bias, having been derived from various sources including the British Constitution, US Constitution, and Irish Constitution.',
    'The Indian Constitution is completely flexible and can be amended by a simple majority in Parliament, similar to ordinary legislation.',
    'The Indian Constitution is purely federal in nature with equal distribution of powers between the Center and States, similar to the US Constitution.',
    'The Indian Constitution is unitary in nature with all powers concentrated in the Central government, similar to the British system.',
    
    'A',
    'The Indian Constitution is described as quasi-federal or federal with a strong unitary bias. It combines federal features like a written constitution, division of powers, and an independent judiciary with unitary features like a strong central government, emergency provisions, and an integrated judiciary. The Constitution can be amended but requires different procedures - some by simple majority, others by special majority, and some by special majority plus ratification by states. This makes it partly rigid and partly flexible.',
    2.00, -- Marks
    0.50, -- NegativeMarks
    'Medium', -- DifficultyLevel
    'MCQ', -- QuestionType
    1, -- SameExplanationForAllLanguages
    'Indian Polity, Constitution Basics', -- Reference
    'Constitution, Fundamental Rights, Federal Structure', -- Tags
    1, -- CreatedBy (Admin)
    1, -- IsPublished
    1 -- IsActive
);

-- Get the QuestionId for the first question
DECLARE @Question1Id INT = SCOPE_IDENTITY();

-- Add Hindi translation for Question 1
INSERT INTO QuestionTranslations (
    QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
)
VALUES (
    @Question1Id,
    'hi',
    '26 नवंबर 1949 को अपनाया गया और 26 जनवरी 1950 को लागू किया गया भारतीय संविधान, भारत का सर्वोच्च कानून है। यह मूल राजनीतिक सिद्धांतों को परिभाषित करने वाले ढांचे को निर्धारित करता है, सरकारी संस्थानों की संरचना, प्रक्रियाएं, शक्तियां और कर्तव्य स्थापित करता है, और मूल अधिकार, नीति सिद्धांत और नागरिकों के कर्तव्यों को निर्धारित करता है। निम्नलिखित में से कौन सा कथन भारतीय संविधान की प्रकृति को सही ढंग से वर्णित करता है?',
    
    'भारतीय संविधान एक कठोर और संघीय संविधान है जिसमें एक मजबूत एकात्मक पूर्वाग्रह है, जिसे विभिन्न स्रोतों जैसे ब्रिटिश संविधान, अमेरिकी संविधान और आयरिश संविधान से प्राप्त किया गया है।',
    'भारतीय संविधान पूरी तरह से लचीला है और संसद में साधारण बहुमत से संशोधित किया जा सकता है, सामान्य विधान के समान।',
    'भारतीय संविधान पूरी तरह से संघीय प्रकृति का है जिसमें केंद्र और राज्यों के बीच शक्तियों का समान वितरण है, अमेरिकी संविधान के समान।',
    'भारतीय संविधान एकात्मक प्रकृति का है जिसमें सभी शक्तियां केंद्र सरकार में केंद्रित हैं, ब्रिटिश प्रणाली के समान।',
    
    'भारतीय संविधान को अर्ध-संघीय या मजबूत एकात्मक पूर्वाग्रह के साथ संघीय के रूप में वर्णित किया गया है। यह संघीय विशेषताओं जैसे लिखित संविधान, शक्तियों का विभाजन, और स्वतंत्र न्यायपालिका को एकात्मक विशेषताओं के साथ जोड़ता है जैसे मजबूत केंद्र सरकार, आपातकालीन प्रावधान, और एकीकृत न्यायपालिका। संविधान को संशोधित किया जा सकता है लेकिन इसके लिए अलग-अलग प्रक्रियाओं की आवश्यकता होती है - कुछ साधारण बहुमत से, अन्य विशेष बहुमत से, और कुछ विशेष बहुमत प्लस राज्यों द्वारा अनुमोदन से। यह इसे आंशिक रूप से कठोर और आंशिक रूप से लचीला बनाता है।'
);

-- Question 2: Fundamental Rights
INSERT INTO Questions (
    ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD, 
    CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType, 
    SameExplanationForAllLanguages, Reference, Tags, CreatedBy, IsPublished, IsActive
)
VALUES (
    1, -- ModuleId
    1, -- ExamId
    7, -- SubjectId
    NULL, -- TopicId
    'Fundamental Rights in the Indian Constitution are enshrined in Part III (Articles 12-35) and are inspired by the US Constitution''s Bill of Rights. These rights are not absolute and are subject to reasonable restrictions. They are enforceable by courts and can be suspended during emergencies (except certain rights). Which of the following pairs correctly matches the Fundamental Right with its corresponding Article?',
    
    'Right to Equality - Articles 14-18, Right to Freedom - Articles 19-22, Right against Exploitation - Articles 23-24',
    'Right to Equality - Articles 15-19, Right to Freedom - Articles 20-24, Cultural and Educational Rights - Articles 29-30',
    'Right to Equality - Articles 12-16, Right to Freedom - Articles 17-21, Right to Constitutional Remedies - Articles 32-35',
    'Right to Equality - Articles 14-18, Right to Freedom - Articles 19-22, Right to Constitutional Remedies - Articles 32-35',
    
    'D',
    'The correct matching is: Right to Equality (Articles 14-18) includes equality before law, prohibition of discrimination, equality of opportunity, and abolition of untouchability. Right to Freedom (Articles 19-22) includes six freedoms, protection in respect of conviction, protection of life and personal liberty, and preventive detention. Right to Constitutional Remedies (Articles 32-35) includes the right to move to Supreme Court for enforcement, and Parliament''s power to amend these rights. Right against Exploitation covers Articles 23-24, Cultural and Educational Rights cover Articles 29-30, and Right to Religion covers Articles 25-28.',
    2.00, -- Marks
    0.50, -- NegativeMarks
    'Medium', -- DifficultyLevel
    'MCQ', -- QuestionType
    1, -- SameExplanationForAllLanguages
    'Indian Polity, Fundamental Rights', -- Reference
    'Fundamental Rights, Articles, Constitution', -- Tags
    1, -- CreatedBy
    1, -- IsPublished
    1 -- IsActive
);

-- Get the QuestionId for the second question
DECLARE @Question2Id INT = SCOPE_IDENTITY();

-- Add Hindi translation for Question 2
INSERT INTO QuestionTranslations (
    QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
)
VALUES (
    @Question2Id,
    'hi',
    'भारतीय संविधान में मूल अधिकार भाग III (अनुच्छेद 12-35) में निहित हैं और अमेरिकी संविधान के बिल ऑफ राइट्स से प्रेरित हैं। ये अधिकार निरपेक्ष नहीं हैं और उचित प्रतिबंधों के अधीन हैं। ये न्यायालयों द्वारा लागू किए जा सकते हैं और आपातकाल के दौरान निलंबित किए जा सकते हैं (कुछ अधिकारों को छोड़कर)। निम्नलिखित में से कौन सा युग्म मूल अधिकार को इसके संबंधित अनुच्छेद के साथ सही ढंग से मेल करता है?',
    
    'समानता का अधिकार - अनुच्छेद 14-18, स्वतंत्रता का अधिकार - अनुच्छेद 19-22, शोषण के विरुद्ध अधिकार - अनुच्छेद 23-24',
    'समानता का अधिकार - अनुच्छेद 15-19, स्वतंत्रता का अधिकार - अनुच्छेद 20-24, सांस्कृतिक और शैक्षिक अधिकार - अनुच्छेद 29-30',
    'समानता का अधिकार - अनुच्छेद 12-16, स्वतंत्रता का अधिकार - अनुच्छेद 17-21, संवैधानिक उपचारों का अधिकार - अनुच्छेद 32-35',
    'समानता का अधिकार - अनुच्छेद 14-18, स्वतंत्रता का अधिकार - अनुच्छेद 19-22, संवैधानिक उपचारों का अधिकार - अनुच्छेद 32-35',
    
    'सही मेलिंग है: समानता का अधिकार (अनुच्छेद 14-18) में कानून के समक्ष समानता, भेदभाव पर प्रतिबंध, अवसरों की समानता, और अस्पृश्यता का उन्मूलन शामिल है। स्वतंत्रता का अधिकार (अनुच्छेद 19-22) में छह स्वतंत्रताएं, दोषसिद्धि के संबंध में सुरक्षा, जीवन और व्यक्तिगत स्वतंत्रता की सुरक्षा, और निवारक निरोध शामिल हैं। संवैधानिक उपचारों का अधिकार (अनुच्छेद 32-35) में लागू करने के लिए सर्वोच्च न्यायालय जाने का अधिकार, और इन अधिकारों को संशोधित करने की संसद की शक्ति शामिल है। शोषण के विरुद्ध अधिकार अनुच्छेद 23-24 को कवर करता है, सांस्कृतिक और शैक्षिक अधिकार अनुच्छेद 29-30 को कवर करते हैं, और धर्म का अधिकार अनुच्छेद 25-28 को कवर करता है।'
);

-- Question 3: Directive Principles
INSERT INTO Questions (
    ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD, 
    CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType, 
    SameExplanationForAllLanguages, Reference, Tags, CreatedBy, IsPublished, IsActive
)
VALUES (
    1, -- ModuleId
    1, -- ExamId
    7, -- SubjectId
    NULL, -- TopicId
    'The Directive Principles of State Policy (DPSP) are contained in Part IV (Articles 36-51) of the Indian Constitution. They are fundamental in the governance of the country and it is the duty of the State to apply these principles in making laws. Unlike Fundamental Rights, DPSPs are not enforceable by courts but are nevertheless fundamental in governance. Which of the following statements about DPSPs is incorrect?',
    
    'DPSPs are inspired by the Irish Constitution and promote the concept of a welfare state.',
    'DPSPs can be classified into three categories: socialist, Gandhian, and liberal-intellectual.',
    'DPSPs are justiciable and can be enforced by courts through writs like Fundamental Rights.',
    'DPSPs aim to establish social and economic democracy rather than merely political democracy.',
    
    'C',
    'The statement that DPSPs are justiciable and can be enforced by courts is incorrect. DPSPs are non-justiciable, meaning they cannot be enforced by courts. However, they are fundamental in governance and the State is duty-bound to apply these principles in making laws. DPSPs are indeed inspired by the Irish Constitution (Article 45 of the Irish Constitution), promote welfare state concepts, and can be classified into socialist (Articles 38-43), Gandhian (Articles 40, 43, 46, 47), and liberal-intellectual (Articles 44, 45, 48, 49, 50) categories. They aim to establish social and economic democracy alongside political democracy.',
    2.00, -- Marks
    0.50, -- NegativeMarks
    'Medium', -- DifficultyLevel
    'MCQ', -- QuestionType
    1, -- SameExplanationForAllLanguages
    'Indian Polity, Directive Principles', -- Reference
    'DPSP, Non-justiciable, Welfare State', -- Tags
    1, -- CreatedBy
    1, -- IsPublished
    1 -- IsActive
);

-- Get the QuestionId for the third question
DECLARE @Question3Id INT = SCOPE_IDENTITY();

-- Add Hindi translation for Question 3
INSERT INTO QuestionTranslations (
    QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
)
VALUES (
    @Question3Id,
    'hi',
    'राज्य के नीति सिद्धांत (DPSP) भारतीय संविधान के भाग IV (अनुच्छेद 36-51) में निहित हैं। ये देश के शासन में मूल हैं और यह राज्य का कर्तव्य है कि वे कानून बनाते समय इन सिद्धांतों को लागू करें। मूल अधिकारों के विपरीत, DPSP न्यायालयों द्वारा लागू नहीं किए जा सकते हैं लेकिन फिर भी शासन में मूल हैं। DPSP के बारे में निम्नलिखित में से कौन सा कथन गलत है?',
    
    'DPSP आयरिश संविधान से प्रेरित हैं और कल्याणकारी राज्य की अवधारणा को बढ़ावा देते हैं।',
    'DPSP को तीन श्रेणियों में वर्गीकृत किया जा सकता है: समाजवादी, गांधीवादी, और उदारवादी-बौद्धिक।',
    'DPSP न्यायसंगत हैं और मूल अधिकारों की तरह रिट्स के माध्यम से न्यायालयों द्वारा लागू किए जा सकते हैं।',
    'DPSP का लक्ष्य केवल राजनीतिक लोकतंत्र के बजाय सामाजिक और आर्थिक लोकतंत्र स्थापित करना है।',
    
    'यह कथन कि DPSP न्यायसंगत हैं और न्यायालयों द्वारा लागू किए जा सकते हैं, गलत है। DPSP गैर-न्यायसंगत हैं, जिसका अर्थ है कि उन्हें न्यायालयों द्वारा लागू नहीं किया जा सकता है। हालांकि, ये शासन में मूल हैं और राज्य का कर्तव्य है कि वे कानून बनाते समय इन सिद्धांतों को लागू करे। DPSP वास्तव में आयरिश संविधान (आयरिश संविधान का अनुच्छेद 45) से प्रेरित हैं, कल्याणकारी राज्य अवधारणाओं को बढ़ावा देते हैं, और समाजवादी (अनुच्छेद 38-43), गांधीवादी (अनुच्छेद 40, 43, 46, 47), और उदारवादी-बौद्धिक (अनुच्छेद 44, 45, 48, 49, 50) श्रेणियों में वर्गीकृत किए जा सकते हैं। इनका लक्ष्य राजनीतिक लोकतंत्र के साथ-साथ सामाजिक और आर्थिक लोकतंत्र स्थापित करना है।'
);

-- Question 4: Parliamentary System
INSERT INTO Questions (
    ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD, 
    CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType, 
    SameExplanationForAllLanguages, Reference, Tags, CreatedBy, IsPublished, IsActive
)
VALUES (
    1, -- ModuleId
    1, -- ExamId
    7, -- SubjectId
    NULL, -- TopicId
    'The Indian parliamentary system is based on the British model and follows the Westminster system of government. The President is the constitutional head of the country while the Prime Minister is the real executive. The system is characterized by the principle of collective responsibility of the Council of Ministers to the Lok Sabha. Which of the following features is NOT a characteristic of the Indian parliamentary system?',
    
    'The President appoints the Prime Minister and other ministers on the advice of the Prime Minister.',
    'The Council of Ministers is collectively responsible to the Lok Sabha and can be removed by a no-confidence motion.',
    'The Prime Minister is the head of the government and enjoys real executive powers.',
    'The President can dismiss the Prime Minister at any time without parliamentary approval.',
    
    'D',
    'The statement that the President can dismiss the Prime Minister at any time without parliamentary approval is NOT a characteristic of the Indian parliamentary system. In the parliamentary system, the President acts on the advice of the Council of Ministers. While the President has certain discretionary powers, dismissing the Prime Minister unilaterally is not one of them. The Prime Minister can only be removed if they lose the confidence of the Lok Sabha through a no-confidence motion. The other options are correct features: the President appoints ministers on PM''s advice, the Council is collectively responsible to Lok Sabha, and the PM enjoys real executive powers as head of government.',
    2.00, -- Marks
    0.50, -- NegativeMarks
    'Medium', -- DifficultyLevel
    'MCQ', -- QuestionType
    1, -- SameExplanationForAllLanguages
    'Indian Polity, Parliamentary System', -- Reference
    'Parliamentary System, President, Prime Minister', -- Tags
    1, -- CreatedBy
    1, -- IsPublished
    1 -- IsActive
);

-- Get the QuestionId for the fourth question
DECLARE @Question4Id INT = SCOPE_IDENTITY();

-- Add Hindi translation for Question 4
INSERT INTO QuestionTranslations (
    QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
)
VALUES (
    @Question4Id,
    'hi',
    'भारतीय संसदीय प्रणाली ब्रिटिश मॉडल पर आधारित है और सरकार की वेस्टमिंस्टर प्रणाली का पालन करती है। राष्ट्रपति देश का संवैधानिक प्रमुख है जबकि प्रधानमंत्री वास्तविक कार्यकारी है। प्रणाली की विशेषता मंत्रिपरिषद की लोकसभा के प्रति सामूहिक जिम्मेदारी के सिद्धांत है। निम्नलिखित में से कौन सी विशेषता भारतीय संसदीय प्रणाली की विशेषता नहीं है?',
    
    'राष्ट्रपति प्रधानमंत्री और अन्य मंत्रियों की नियुक्ति प्रधानमंत्री की सलाह पर करते हैं।',
    'मंत्रिपरिषद लोकसभा के प्रति सामूहिक रूप से जिम्मेदार है और अविश्वास प्रस्ताव द्वारा हटाई जा सकती है।',
    'प्रधानमंत्री सरकार के प्रमुख हैं और वास्तविक कार्यकारी शक्तियों का आनंद लेते हैं।',
    'राष्ट्रपति किसी भी समय संसदीय अनुमोदन के बिना प्रधानमंत्री को बर्खास्त कर सकते हैं।',
    
    'यह कथन कि राष्ट्रपति किसी भी समय संसदीय अनुमोदन के बिना प्रधानमंत्री को बर्खास्त कर सकते हैं, भारतीय संसदीय प्रणाली की विशेषता नहीं है। संसदीय प्रणाली में, राष्ट्रपति मंत्रिपरिषद की सलाह पर कार्य करते हैं। जबकि राष्ट्रपति के पास कुछ विवेकाधीन शक्तियां हैं, प्रधानमंत्री को एकपक्षीय रूप से बर्खास्त करना उनमें से एक नहीं है। प्रधानमंत्री को केवल तभी हटाया जा सकता है जब वे अविश्वास प्रस्ताव के माध्यम से लोकसभा के विश्वास को खो देते हैं। अन्य विकल्प सही विशेषताएं हैं: राष्ट्रपति पीएम की सलाह पर मंत्रियों की नियुक्ति करते हैं, मंत्रिपरिषद लोकसभा के प्रति सामूहिक रूप से जिम्मेदार है, और पीएम सरकार के प्रमुख के रूप में वास्तविक कार्यकारी शक्तियों का आनंद लेते हैं।'
);

-- Question 5: Judicial System
INSERT INTO Questions (
    ModuleId, ExamId, SubjectId, TopicId, QuestionText, OptionA, OptionB, OptionC, OptionD, 
    CorrectAnswer, Explanation, Marks, NegativeMarks, DifficultyLevel, QuestionType, 
    SameExplanationForAllLanguages, Reference, Tags, CreatedBy, IsPublished, IsActive
)
VALUES (
    1, -- ModuleId
    1, -- ExamId
    7, -- SubjectId
    NULL, -- TopicId
    'The Indian judicial system is an integrated and independent judiciary established under Part V of the Constitution. The Supreme Court is the apex court, followed by High Courts and subordinate courts. The Constitution provides for the appointment of judges through a collegium system and ensures their independence through security of tenure and fixed service conditions. Which of the following statements regarding the appointment and removal of Supreme Court judges is correct?',
    
    'Supreme Court judges are appointed by the President on the recommendation of the Prime Minister and can be removed by Parliament through impeachment.',
    'Supreme Court judges are appointed by the President after consultation with the judiciary and can be removed by the President on grounds of proved misbehavior.',
    'Supreme Court judges are appointed by the President on the recommendation of the collegium system and can be removed by Parliament through impeachment for proven misbehavior or incapacity.',
    'Supreme Court judges are appointed by the Prime Minister and can be removed by the Supreme Court itself through an internal disciplinary process.',
    
    'C',
    'The correct statement is that Supreme Court judges are appointed by the President on the recommendation of the collegium system (comprising the Chief Justice of India and senior Supreme Court judges) and can be removed by Parliament through impeachment for proven misbehavior or incapacity. The appointment process involves consultation with the judiciary as per Article 124, and the removal process requires a special majority in both Houses of Parliament and the signature of the President (Articles 124(4)). The removal process is rigorous and has never been successfully completed in India''s history, ensuring judicial independence.',
    2.00, -- Marks
    0.50, -- NegativeMarks
    'Medium', -- DifficultyLevel
    'MCQ', -- QuestionType
    1, -- SameExplanationForAllLanguages
    'Indian Polity, Judicial System', -- Reference
    'Supreme Court, Collegium System, Impeachment', -- Tags
    1, -- CreatedBy
    1, -- IsPublished
    1 -- IsActive
);

-- Get the QuestionId for the fifth question
DECLARE @Question5Id INT = SCOPE_IDENTITY();

-- Add Hindi translation for Question 5
INSERT INTO QuestionTranslations (
    QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation
)
VALUES (
    @Question5Id,
    'hi',
    'भारतीय न्यायिक प्रणाली संविधान के भाग V के तहत स्थापित एक एकीकृत और स्वतंत्र न्यायपालिका है। सर्वोच्च न्यायालय शीर्ष न्यायालय है, जिसके बाद उच्च न्यायालय और अधीनस्थ न्यायालय हैं। संविधान कॉलेजियम प्रणाली के माध्यम से न्यायाधीशों की नियुक्ति का प्रावधान करता है और पद की सुरक्षा और निश्चित सेवा शर्तों के माध्यम से उनकी स्वतंत्रता सुनिश्चित करता है। सर्वोच्च न्यायालय के न्यायाधीशों की नियुक्ति और पदमुक्ति के संबंध में निम्नलिखित में से कौन सा कथन सही है?',
    
    'सर्वोच्च न्यायालय के न्यायाधीशों की नियुक्ति राष्ट्रपति द्वारा प्रधानमंत्री की सिफारिश पर की जाती है और उन्हें संसद द्वारा महाभियोग के माध्यम से हटाया जा सकता है।',
    'सर्वोच्च न्यायालय के न्यायाधीशों की नियुक्ति राष्ट्रपति द्वारा न्यायपालिका से परामर्श के बाद की जाती है और उन्हें राष्ट्रपति द्वारा सिद्ध दुर्व्यवहार के आधार पर हटाया जा सकता है।',
    'सर्वोच्च न्यायालय के न्यायाधीशों की नियुक्ति राष्ट्रपति द्वारा कॉलेजियम प्रणाली की सिफारिश पर की जाती है और उन्हें संसद द्वारा सिद्ध दुर्व्यवहार या अक्षमता के लिए महाभियोग के माध्यम से हटाया जा सकता है।',
    'सर्वोच्च न्यायालय के न्यायाधीशों की नियुक्ति प्रधानमंत्री द्वारा की जाती है और उन्हें सर्वोच्च न्यायालय द्वारा आंतरिक अनुशासनात्मक प्रक्रिया के माध्यम से हटाया जा सकता है।',
    
    'सही कथन यह है कि सर्वोच्च न्यायालय के न्यायाधीशों की नियुक्ति राष्ट्रपति द्वारा कॉलेजियम प्रणाली (जिसमें भारत के मुख्य न्यायाधीश और वरिष्ठ सर्वोच्च न्यायालय के न्यायाधीश शामिल हैं) की सिफारिश पर की जाती है और उन्हें संसद द्वारा सिद्ध दुर्व्यवहार या अक्षमता के लिए महाभियोग के माध्यम से हटाया जा सकता है। नियुक्ति प्रक्रिया में अनुच्छेद 124 के अनुसार न्यायपालिका से परामर्श शामिल है, और पदमुक्ति प्रक्रिया के लिए संसद के दोनों सदनों में विशेष बहुमत और राष्ट्रपति के हस्ताक्षर की आवश्यकता होती है (अनुच्छेद 124(4))। पदमुक्ति प्रक्रिया कड़ी है और भारत के इतिहास में कभी सफलतापूर्वक पूरी नहीं हुई है, जो न्यायिक स्वतंत्रता सुनिश्चित करती है।'
);

PRINT 'Lengthy Questions with Hindi Translations Created Successfully!';
PRINT 'Question IDs Created:';
PRINT 'Question 1 ID: ' + CAST(@Question1Id AS VARCHAR);
PRINT 'Question 2 ID: ' + CAST(@Question2Id AS VARCHAR);
PRINT 'Question 3 ID: ' + CAST(@Question3Id AS VARCHAR);
PRINT 'Question 4 ID: ' + CAST(@Question4Id AS VARCHAR);
PRINT 'Question 5 ID: ' + CAST(@Question5Id AS VARCHAR);
