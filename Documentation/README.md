# Semantic Similarity Analysis of Textual Data

## Overview
This project analyzes the semantic similarity between textual data using OpenAI embeddings. Currently it compares input text files with different domain text files and calculates similarity scores using cosine similarity.

## Features
- Reads text documents from an input folder.
- Generates embeddings using OpenAI API.
- Computes similarity scores between input texts and domains.
- Exports similarity results to a CSV file.

## Installation
### Prerequisites
- .NET 9.0
- OpenAI API Key

### Setup
1. **Clone the repository**
   ```sh
   git clone https://github.com/nishta168/TextSimilarityAnalyser.git
   cd TextSimilarityAnalyzer
   ```
2. **Install dependencies**
   ```sh
   dotnet restore
   ```
3. **Set up the OpenAI API key**
   - Add the API key to `appsettings.json`:
     ```json
     {
       "OpenAI": {
         "ApiKey": "your-api-key-here"
       }
     }
     ```
   - Alternatively, set it as an environment variable:
     ```sh
     setx OPENAI_API_KEY "your-api-key-here"
     ```

## Usage

### Expected Input
- Text files should be placed in the `input_texts/` directory.
- Domain-specific text files should be placed in the `domains/` directory.

### Output
- The program generates a CSV file `similarity_result.csv` in the `output/` folder containing similarity scores.

## Troubleshooting
- **Issue: API Key Error**
  - Ensure the API key is correctly set in `appsettings.json` or as an environment variable.
- **Issue: No files detected**
  - Verify that input and domain folders contain `.txt` files.
- **Issue: Output folder error**
  - Manually create an `output/` folder if the program fails to generate results.


