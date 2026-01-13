# Hotel Booking System

A complete online hotel booking system built with ASP.NET Core 9.0 and Entity Framework Core.

## 📋 Table of Contents

- [About](#-about)
- [Technologies](#-technologies)
- [Features](#-features)
- [Project Architecture](#-project-architecture)
- [Database Structure](#-database-structure)
- [Installation](#-installation)
- [Usage](#-usage)
- [Default Admin Credentials](#-default-admin-credentials)
- [Security](#-security)

## 🎯 About

Hotel Booking System is a modern web platform for managing hotel reservations. The application allows users to search, view, and book hotel rooms from different locations, providing a complete e-commerce experience for the hospitality industry.

## 🛠 Technologies

### Backend
- **Framework**: ASP.NET Core 9.0 (MVC)
- **Language**: C# 13.0
- **ORM**: Entity Framework Core 9.0.4
- **Database**: SQL Server (LocalDB)
- **Authentication**: ASP.NET Core Identity

### Frontend
- **View Engine**: Razor Views
- **CSS Framework**: Bootstrap 5
- **Icons**: Bootstrap Icons
- **JavaScript**: jQuery + Vanilla JS

### Integrations
- **PayPal SDK**: PayPalCheckoutSdk 1.0.4 for payment processing
- **Microsoft.EntityFrameworkCore.SqlServer**: 9.0.4
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore**: 9.0.4

## ✨ Features

### User Features
- ✅ **Authentication & Registration**: Complete account management system
- ✅ **Room Search**: Filter by location, room type, and price
- ✅ **Room Details**: Photo galleries, detailed descriptions, pricing
- ✅ **Shopping Cart**: Add multiple rooms with different dates
- ✅ **Favorites List**: Save preferred rooms for quick access
- ✅ **Booking System**: 
  - Select check-in/check-out dates
  - Automatic calculation of nights and total price
  - Availability validation
- ✅ **Online Payments**: PayPal integration for secure payments
- ✅ **Booking History**: View and manage previous bookings
- ✅ **User Profile**: 
  - Edit personal information
  - Upload profile pictures
  - View activity history

### Admin Features
- ✅ **Admin Dashboard**: Control panel with statistics
- ✅ **Room Management**:
  - Create, edit, delete rooms
  - Upload multiple photos
  - Set pricing and availability
- ✅ **Location Management**: Manage hotel locations
- ✅ **Booking Overview**: Monitor all bookings
- ✅ **User Management**: Administer user accounts

## 🏗 Project Architecture

The project follows a Layered Architecture with clear separation of concerns:

```
Hotel/
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── RoomController.cs
│   ├── BookingController.cs
│   ├── CartController.cs
│   ├── FavoriteController.cs
│   ├── LocationController.cs
│   └── UsersController.cs
│
├── Models/              # Database Entities
│   ├── User.cs
│   ├── Room.cs
│   ├── Booking.cs
│   ├── Cart.cs
│   ├── Favorite.cs
│   ├── Location.cs
│   └── Context/
│       └── HotelContext.cs
│
├── ViewModels/          # View Models
│   ├── BookingViewModel.cs
│   ├── RoomViewModel.cs
│   ├── ProfileModel.cs
│   ├── LoginModel.cs
│   ├── SignModel.cs
│   └── AdminDashBordModel.cs
│
├── Services/            # Business Logic Layer
│   ├── Room/
│   ├── Booking/
│   ├── Cart/
│   ├── Favorite/
│   ├── Locations/
│   ├── User/
│   └── PayPal/
│
├── Data/                # Repository Pattern - Data Access
│   ├── Room/
│   ├── Booking/
│   ├── Cart/
│   ├── Favorite/
│   └── Locations/
│
├── Views/               # Razor Views
│   ├── Home/
│   ├── Room/
│   ├── Booking/
│   ├── Cart/
│   ├── Favorite/
│   ├── Location/
│   ├── Users/
│   └── Shared/
│
├── wwwroot/            # Static Files
│   ├── css/
│   ├── js/
│   ├── images/
│   └── room-photos/
│
├── Migrations/         # EF Core Migrations
├── Program.cs          # Application Entry Point
└── appsettings.json    # Configuration
```

## 💾 Database Structure

### Main Tables

#### Users (AspNetUsers)
Extends `IdentityUser` with:
- `Name`: Full name
- `Phone`: Phone number
- `ProfilePicturePath`: Profile picture path

#### Rooms
- `Id`: Unique identifier
- `Type`: Room type (Single, Double, Suite, etc.)
- `Price`: Price per night
- `RoomCount`: Number of available rooms
- `Description`: Detailed description
- `MainPhotoPath`: Main photo
- `LocationId`: Location reference
- **Relations**: `Photos` (1-to-many), `Location` (many-to-1)

#### Bookings
- `Id`: Booking identifier
- `UserId`: User reference
- `CheckInDate`: Check-in date
- `CheckOutDate`: Check-out date
- `BookingDate`: Booking creation date
- `Status`: Booking status (Pending, Confirmed, Cancelled, Completed)
- **Relations**: `BookingItems` (1-to-many)

#### BookingItems
- Links bookings with rooms
- `Quantity`: Number of rooms booked
- `PricePerNight`: Price snapshot per night

#### Cart & CartItems
- Persistent shopping cart system
- Stores check-in/check-out dates for each item

#### Favorites
- User favorite rooms list
- Many-to-many relationship between Users and Rooms

#### Locations
- `Id`: Location identifier
- `Name`: Location name (city/region)

## 🚀 Installation

### Prerequisites

- **.NET 9.0 SDK** or newer
- **SQL Server** (LocalDB or full version)
- **Visual Studio 2022** (recommended) or Visual Studio Code
- **Git** for version control

## 📱 Usage

### For Customers

1. **Registration/Login**
   - Access the Sign Up page
   - Complete the registration form
   - Verify email (if confirmation is enabled)

2. **Search Rooms**
   - Navigate to Rooms section
   - Filter by location and preferences
   - View details and photos

3. **Make a Booking**
   - Select desired rooms
   - Choose check-in/check-out dates
   - Add to cart
   - Complete order and pay with PayPal

4. **Account Management**
   - Access profile to edit information
   - View booking history
   - Manage favorites list

### For Administrators

1. **Login**
   - Use admin credentials

2. **Dashboard**
   - View general statistics
   - Monitor recent bookings

3. **Room Management**
   - Create new rooms with complete details
   - Upload multiple photos
   - Edit prices and availability
   - Delete inactive rooms

4. **Location Management**
   - Add new hotel locations
   - Edit existing locations

## 🔑 Default Admin Credentials

On first run, the system automatically creates an administrator account:

- **Email**: admin@hotel.com
- **Password**: Admin123!


## 🔒 Security

- **HTTPS** required in production
- **Identity Framework** for authentication
- **Role-Based Authorization** (Admin, User)
- **Anti-Forgery Tokens** on all forms
- **SQL Injection Protection** through Entity Framework
- **XSS Protection** through automatic Razor encoding

## 🎨 Key Features

### Photo System
- Multiple photo uploads per room
- Set featured/main photo
- Photo gallery with slider on details page

### Validations
- Server-side and client-side validation
- Room availability checking
- Check-in/check-out date validation
- PayPal form validation

### Performance
- Eager Loading for relationships
- Optional Lazy Loading
- Static data caching
- Optimized LINQ queries

## 📄 License

This project is developed for educational purposes.
