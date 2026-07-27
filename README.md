# 🐾 Veterinary Automation System

A comprehensive desktop-based veterinary clinic management system developed with **C# Windows Forms** and **Microsoft SQL Server**. The application provides an integrated solution for managing animal records, inventory, customer debts, pregnancy tracking, notifications, and clinic operations through a centralized and user-friendly interface.

The system was designed to help veterinary clinics reduce manual processes, improve data organization, and efficiently manage day-to-day operations.

---

## 🚀 Key Features

### 🐾 Animal Registration & Management

* Create and manage detailed animal records.
* Store owner information, ear tag number (`KupeNo`), address, and animal species.
* Validate user input, including phone number format validation.
* Maintain relationships between animal records and other modules.
* Automatically synchronize animal data with the pregnancy tracking system.

---

### 📦 Inventory & Stock Management

* Full **CRUD** operations for inventory items.
* Manage products such as veterinary medicines, animal feed, and other supplies.
* Automatically calculate profit based on:

```text
Profit = (Sale Price - Purchase Price) × Quantity
```

* Filter products dynamically by name and price.
* Edit inventory records directly through `DataGridView`.
* Apply changes directly to the SQL Server database.

---

### 💰 Customer Debt & Payment Management

* Track customer debts and payment transactions.
* Automatically update outstanding balances after payments.
* Search customers by name or phone number.
* Support multiple payment methods, including cash and card.
* Maintain a centralized transaction history for customer accounts.

---

### 🤰 Pregnancy Tracking

* Create and manage pregnancy records for animals.
* Automatically calculate expected birth dates.
* Monitor upcoming birth dates through a color-coded status system.

#### Status Indicators

* 🟥 **Red:** Less than 15 days remaining until the expected birth date.
* 🟩 **Green:** More than 15 days remaining.
* ⚪ **White:** Unknown or unavailable status.

The system can also send automatic email notifications when an expected birth date is approaching.

---

## 🔔 Notifications & Automation

The application includes an automated notification system designed to keep veterinary staff informed about important events.

* Automatic email notifications for upcoming births.
* System tray notifications using `NotifyIcon`.
* Notification sounds for important alerts.
* Background timer-based data refresh.
* Automated monitoring of pregnancy due dates.
* Email configuration managed through the application database.

---

## 🛠️ Technical Overview

| Technology                | Usage                            |
| ------------------------- | -------------------------------- |
| **C#**                    | Core programming language        |
| **Windows Forms**         | Desktop application framework    |
| **.NET / .NET Framework** | Application development platform |
| **Microsoft SQL Server**  | Relational database management   |
| **System.Data.SqlClient** | Database connectivity            |
| **System.Net.Mail**       | Email notification services      |
| **System.Media**          | Notification sounds              |
| **NotifyIcon**            | System tray notifications        |
| **Visual Studio**         | Development environment          |

---

## 🗃️ Database Structure

The application uses **Microsoft SQL Server** for persistent data storage.

### Main Tables

* `HayvanKayit` — Animal and owner information
* `Gebelik` — Pregnancy tracking and due dates
* `stok` — Inventory, product, and profit information
* `Islemler` — Customer debt and payment transactions
* `mail` — Email and SMTP configuration

---

## ⚙️ System Architecture

The application follows a modular structure in which different clinic operations are managed through dedicated modules.

```text
Veterinary Automation System
│
├── Animal Management
│   ├── Animal Registration
│   └── Owner Information
│
├── Pregnancy Tracking
│   ├── Due Date Calculation
│   └── Automated Notifications
│
├── Inventory Management
│   ├── Product Management
│   ├── Stock Tracking
│   └── Profit Calculation
│
├── Debt Management
│   ├── Customer Accounts
│   ├── Payment Tracking
│   └── Balance Management
│
└── Notification System
    ├── Email Notifications
    ├── System Tray Alerts
    └── Sound Notifications
```

---

## 💻 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR-USERNAME/Veterinary-Automation-System.git
```

### 2. Open the Project

Open the solution in **Microsoft Visual Studio**.

### 3. Configure the Database

* Restore the provided SQL Server database or execute the included SQL scripts.
* Configure the database connection string in `App.config`.
* Replace the server name with your local SQL Server instance.

Example:

```xml
<connectionStrings>
    <add name="VeterinaryDb"
         connectionString="YOUR_CONNECTION_STRING"
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

### 4. Build & Run

Build the solution and run the application through Visual Studio.

---

## 📸 Screenshots

> Screenshots of the application interface will be added here.

---

## 🎯 Project Goals

The primary goal of this project is to provide veterinary clinics with a centralized digital management solution that simplifies daily operations and reduces manual paperwork.

The system brings multiple operational processes together in a single application, helping improve:

* Data organization
* Inventory management
* Customer account tracking
* Animal record management
* Pregnancy monitoring
* Automated notifications
* Operational efficiency

---

## 🎓 Project Background

This project was developed as a comprehensive veterinary clinic automation system for **Ondokuz Mayıs University (OMÜ)**.

The project was designed and developed by **Samet Akman** using C#, Windows Forms, and Microsoft SQL Server.

---

## 👨‍💻 Developer

**Samet Akman**

Computer Programming | C# & .NET Developer

---

## 🏷️ Technologies & Keywords

`C#` `.NET` `WinForms` `SQL Server` `Desktop Application` `Veterinary Software` `Clinic Management` `Database Management` `CRUD` `Inventory Management` `Pregnancy Tracking` `Debt Management` `Email Notifications` `Automation`
