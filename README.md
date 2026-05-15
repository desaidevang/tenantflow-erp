# TenantFlow ERP

A full-stack Multi-Tenant ERP SaaS platform built with ASP.NET Core Web API, PostgreSQL, and React, focused on scalable business workflows, tenant isolation, inventory management, invoice generation, and role-based access control.

---

# Project Overview

TenantFlow ERP is designed as a SaaS-style ERP platform where multiple companies (tenants) can independently manage their products, inventory, customers, invoices, and staff within isolated environments.

The platform includes:

* Multi-Tenant SaaS Architecture
* JWT Authentication & Role-Based Authorization
* Inventory Ledger Management
* Invoice Management with PDF Generation
* Dashboard Analytics APIs
* Search & Pagination
* Platform Admin Approval Flow
* React Admin Dashboard

---

# Tech Stack

## Backend

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* BCrypt Password Hashing
* QuestPDF
* Swagger / OpenAPI
* Docker (setup in progress)

## Frontend

* React
* Tailwind CSS
* Axios
* React Router

---

# Core Features

## SaaS Multi-Tenant Architecture

* Global Tenant Query Filters
* Tenant-based data isolation
* Platform Admin management
* Tenant approval & activation system

## Authentication & Authorization

* JWT-based authentication
* BCrypt password hashing
* Role-Based Access Control
* Platform Admin / Admin / Staff roles

## Inventory Management

* Product management
* Category management
* Stock-In workflows
* Sales inventory deduction
* Damaged stock tracking
* Returned stock management
* Inventory transaction ledger

## Invoice System

* Invoice creation
* Invoice history
* Invoice PDF generation using QuestPDF
* Invoice item management

## Customer Management

* Customer CRUD operations
* Search functionality
* Pagination support

## Dashboard & Analytics

* Revenue analytics
* Invoice statistics
* Customer statistics
* Product statistics
* Low stock analytics

## Search & Scalability

* Universal Search APIs
* Module-specific search
* Pagination APIs
* Scalable API response structure

## Middleware & Architecture

* Global Exception Middleware
* Service-based architecture
* DTO pattern
* Clean controller structure

---

# Backend Architecture

The backend follows a layered structure:

```text
Controllers → Services → Entity Framework Core → PostgreSQL
```

Project structure:

```text
TenantFlowERP/
│
├── Controllers
├── DTOs
├── Data
├── Entities
├── Interfaces
├── Middleware
├── Migrations
├── Services
```

---

# Security Features

* JWT Authentication
* BCrypt Password Hashing
* Tenant Isolation
* Role-Based Authorization
* Platform Admin Approval Flow
* Protected API Endpoints

---

# API Modules

* Authentication
* Platform Administration
* Products
* Categories
* Customers
* Users
* Invoices
* Inventory Transactions
* Dashboard Analytics
* Search

---

# Frontend Dashboard

The React frontend dashboard includes:

* Login System
* Dashboard Analytics Cards
* Product Management UI
* Customer Management UI
* Invoice Management
* Search Functionality
* Pagination
* PDF Invoice Download
* Role-Based Navigation

---

# Demo Video

A complete walkthrough/demo video of the platform showcasing:

* Authentication
* Tenant workflows
* Inventory system
* Invoice generation
* Dashboard analytics
* Search & pagination
* Role-based access
* Multi-tenant functionality

https://www.linkedin.com/posts/devang-desai-_dotnet-aspnetcore-csharp-ugcPost-7461001836481875969-f7J6

---

# Future Improvements

* Redis Caching
* Docker Deployment
* Cloud Deployment
* Email Notifications
* Advanced Reporting
* Audit Logs
* Payment Integration

---

# Learning Outcomes

This project focused heavily on designing connected business workflows instead of isolated CRUD APIs, emphasizing:

* SaaS architecture
* scalable backend design
* tenant isolation
* workflow-driven systems
* enterprise API structure

---

# Repository Status

Backend completed ✅
Frontend dashboard completed ✅

