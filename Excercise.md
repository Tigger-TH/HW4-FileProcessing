# 🧪 Exercise Specification

## 🎯 Objective

Enhance the Text/CSV Viewer project by:

* Adding interactive features
* Practicing structured programming
* Working with real-world datasets
* Strengthening Git workflow

***

# 🧩 Part 1 — Feature Enhancement

Students must extend the application by adding the following functions:

***

## ✅ 1. Record Navigation

Implement navigation features to move through the data.

### Requirements:

* Add buttons or commands:
  * `Next`
  * `Previous`

### Behavior:

* **Next**
  * Move to the next record in the dataset
* **Previous**
  * Move to the previous record

### Key concepts to practice:

* Index management
* Boundary checking:
  ```text
  Cannot go beyond first or last record
  ```

***

## ✅ 2. Record Display

Improve how data is shown.

### Requirements:

* Display one record at a time OR highlight the selected row
* Show key fields such as:
  * `file_name`
  * `file_type`
  * `signature`

### Key concepts:

* Data binding (UI or console formatting)
* Clean output design

***

## ✅ 3. Search Function

Implement a simple search feature.

### Requirements:

* Allow user to input a keyword
* Search within:
  * file name
  * file type
  * signature

### Example:

```text
Search: "exe"
→ Show all records containing "exe"
```

***

### Suggested Implementation:

```csharp
if (line.Contains(keyword))
{
    // show result
}
```

***

### Key concepts:

* String matching
* Iteration over dataset
* Filtering results

***

## ✅ 4. List View (Optional but Recommended)

Display multiple records in a list/table format.

### Possible features:

* Show first 10–20 records
* Display index number
* Allow selection

***

### Key concepts:

* Loop + formatted output
* Basic UI or structured console output

***

# 🌐 Part 2 — Load Full Dataset from Source

## ✅ Objective

Students will work with **real-world data sources**.

***

## ✅ Task

1. Go to the official MalwareBazaar website:

   👉 <https://bazaar.abuse.ch/>

2. Locate dataset export or sample CSV (follow Terms of Use)

3. Download a dataset file

***

## ✅ Update Your Program

Modify the program to:

* Load the **full dataset file**
* Handle larger data size
* Ensure program remains stable

***

## ⚠️ Important Requirements

* Do NOT include full dataset in your submission repository
* Use it locally for testing only
* Respect the Terms of Use:
  👉 <https://bazaar.abuse.ch/faq/#tos>

***

## ✅ Key concepts:

* Handling large files
* Performance awareness
* External data usage

***

# ⚠️ Ethical Reminder (Important)

This dataset contains malware-related metadata.

Students must:

* Use data for **learning and analysis only**
* NOT attempt to download or execute malware
* Follow ethical and legal guidelines

***

# 🔧 Part 3 — Git Practice (Required)

Students must use Git during development.

***

## ✅ Required Steps

1. Initialize your own repository
2. Commit your progress regularly

Example:

```bash
git add .
git commit -m "Add next/previous navigation"
```

***

## ✅ Suggested Commit Structure

* `Initial project setup`
* `Add CSV loading`
* `Implement navigation feature`
* `Add search functionality`
* `Improve UI display`

***

## ✅ Learning Goal

Understand:

```
Working → Commit → Track changes → Improve incrementally
```

***

# ✅ Submission Requirements

Students should submit:

* Their own Git repository link
* Working project code
* Short description of implemented features

***

# 📊 Evaluation Criteria (Optional for Instructor)

| Criteria      | Description               |
| ------------- | ------------------------- |
| Functionality | Features work correctly   |
| Code Quality  | Clean, readable code      |
| Git Usage     | Meaningful commits        |
| UI/Output     | Clear display             |
| Understanding | Demonstrates key concepts |

***

# 💡 Bonus Challenges (Advanced Students)

* Add sorting (by date, file type)
* Add filtering by multiple conditions
* Load data asynchronously (advanced)
* Build GUI with WinForms/WPF

***

# ✅ Final Note to Students

> This exercise is designed to simulate real software development:
> you will read data, process it, improve features, and manage your work using Git.

***

