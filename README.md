# Budget Tracker (ASP.NET Core MVC)

![ASP.NET](https://img.shields.io/badge/ASP.NET-Core-blue)
![Azure](https://img.shields.io/badge/Azure-App_Service-blue)
![License](https://img.shields.io/badge/license-MIT-green)

**Live App:**  
https://mmueen-budgeting-app.azurewebsites.net/

---

## Overview

Budget Tracker is a full-stack ASP.NET Core MVC web application designed to help users manage personal finances through structured expense tracking, income monitoring, and category-based budgeting.

Users can record financial activity, define spending categories, assign monthly budgets, and monitor trends through a dashboard interface.

This project demonstrates full-stack web development using **C#, ASP.NET Core MVC, Entity Framework Core, and Azure SQL.**

---

## Features

### Financial Tracking
- Create, edit, and delete income records
- Track expenses by category
- View monthly financial summaries
- Automatic calculation of net income

### Budget Management
- Define spending budgets for each category
- Visual progress indicators for budget usage
- Warning states when spending approaches limits

### Dashboard Analytics
- Monthly income vs expense overview
- Net financial balance display
- Six-month trend chart for income and expenses
- Budget vs actual spending visualization

### Recurring Transactions
- Create recurring expense templates
- Automatic generation of expenses based on schedule
- Supports weekly and monthly intervals

### User Experience
- Clean Bootstrap-based UI
- Responsive layout
- Clear financial indicators and status warnings
- Navigation optimized for quick financial review

---

## Technology Stack

### Backend
- ASP.NET Core MVC
- C#
- Entity Framework Core
- LINQ

### Database
- Azure SQL Database
- Code-First Entity Framework migrations

### Frontend
- Razor Views
- Bootstrap 5
- Chart.js

### Hosting / Cloud
- Azure App Service
- Azure SQL Database

### Development Tools
- Visual Studio
- Git
- GitHub

---

## Architecture

This project follows the **Model-View-Controller (MVC)** design pattern.

### Models
Represent financial entities, define relationships, and enforce validation rules.

### Controllers
Handle HTTP requests, query the database using Entity Framework Core, and prepare ViewModels for rendering.

### Views
Render the user interface using Razor templates and display financial summaries and charts.

---

## System Architecture

The application follows a traditional ASP.NET Core MVC layered architecture.

Browser
   │
   ▼
ASP.NET Core MVC
   │
   ├── Controllers
   │       │
   │       ▼
   │   Entity Framework Core
   │       │
   ▼       ▼
Views   ApplicationDbContext
   │       │
   ▼       ▼
 Razor   Azure SQL Database

 ---

## Project Structure
{content: 
BudgetingApp
│
├── Controllers
│ ├── DashboardController
│ ├── ExpenseController
│ ├── IncomeController
│ ├── CategoryController
│ ├── BudgetsController
│ └── RecurringExpenseController
│
├── Models
│ ├── Expense
│ ├── Income
│ ├── Category
│ ├── Budget
│ └── RecurringExpense
│
├── Data
│ └── ApplicationDbContext
│
├── Views
│ ├── Dashboard
│ ├── Expense
│ ├── Income
│ ├── Category
│ ├── Budgets
│ └── RecurringExpense
│
└── wwwroot

}
---

## Database Design

| Table | Purpose |
|------|------|
| Income | Records income transactions |
| Expense | Tracks spending transactions |
| Category | Defines expense categories |
| Budget | Monthly budget allocations |
| RecurringExpense | Scheduled recurring expenses |

The database is managed using **Entity Framework Core migrations**.

---

## Running the Project Locally

Clone the repository: 
git clone https://github.com/mariammueenie/COMP2084-ProjectRepo-BudgettingApp

Navigate to the project folder:
cd BudgetingApp


Restore dependencies:
dotnet restore

Apply migrations:
dotnet ef database update

Run the application:
dotnet run

Open the application in your browser:
https://localhost:xxxx

---

## Key Learning Outcomes

Through this project I developed experience with:

- Designing full-stack web applications using ASP.NET Core MVC
- Implementing relational data models with Entity Framework Core
- Writing LINQ queries to perform database operations
- Structuring applications using the Model-View-Controller pattern
- Deploying production applications to Microsoft Azure
- Managing database schema changes using EF Core migrations
- Building responsive UI layouts using Bootstrap
- Visualizing data using Chart.js

---

## Future Improvements

Potential future enhancements include:

- Multi-user financial isolation
- Data export (CSV / Excel)
- Advanced analytics and spending insights
- Savings goal tracking
- Mobile dashboard optimizations

---

## Author

**Mariam Mueen**  
Computer Programming Student  
Georgian College

GitHub:  
https://github.com/mariammueenie

---

## Course Context

Developed for:

**COMP2084 – Server-Side Scripting (ASP.NET Core MVC)**

This project demonstrates:

- MVC architecture
- Entity Framework Core integration
- relational database design
- database-driven web applications
- Azure cloud deployment
- full-stack web application development