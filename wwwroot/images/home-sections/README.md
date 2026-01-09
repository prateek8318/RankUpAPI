# Home Section Images

Yahan Figma se export kiye gaye images ko store karein.

## Folder Structure:
```
wwwroot/
  images/
    home-sections/
      mock-test-icon.png
      practice-daily.png
      test-series-icon.png
      previous-year-icon.png
      ...
```

## Image URLs:
Jab tum images yahan upload karoge, to API me use karne ke liye URL format:

```
http://localhost:5234/images/home-sections/filename.png
```

## Example:
Agar tum `mock-test-icon.png` upload karte ho, to API me:
```json
{
  "imageUrl": "http://localhost:5234/images/home-sections/mock-test-icon.png"
}
```

## Figma se Export kaise karein:
1. Figma me image select karo
2. Right sidebar me Export section
3. Format: PNG (2x ya 3x size)
4. Export button click karo
5. File ko yahan save karo
