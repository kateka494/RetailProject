Project Overview
This project is a complete retail management web application built using ASP.NET Core MVC. It successfully demonstrates the real-world integration of all four Azure Storage services: Tables, Blob Storage, Queues, and Files.

The application allows users to manage products and customers, upload and display images securely, process orders asynchronously using queues, and maintain a persistent cloud-based log file.

Core Application Features
Product Management:
The application provides full Create, Read, Update, and Delete functionality for products. Each product stores information such as the product name, category, price, description, and a link to its associated image stored in Azure.

Customer Management:
Administrators can register new customers, view a complete list of all registered customers, and remove customers from the system if necessary.

Asynchronous Order Processing:
When a customer clicks the "Queue Order" button on any product, the application does not process the order immediately. Instead, it sends a lightweight data message to an Azure Queue. This allows the user interface to respond instantly, while the actual processing happens in the background.

Secure Image Hosting:
All product images are stored in a private Azure Blob Storage container. To ensure security, the application dynamically generates temporary secure access tokens for each image. This allows the images to be viewed on the website without making the entire storage container publicly accessible.

Cloud-Based Application Logging:
Every time an order is placed, the application writes a detailed log entry directly to a text file stored in an Azure File Share. The website includes a dedicated page that reads this log file and displays it to the administrator in real time.

The Four Azure Storage Services Used
Azure Table Storage
This service is used to store structured data. The application uses two tables: one for storing customer records and another for storing product details. Data is stored and retrieved in a highly scalable NoSQL format.

Azure Blob Storage
This service is used for storing large, unstructured data, specifically product images. All images are kept in a private container to restrict public access, and images are served securely using temporary access tokens.

Azure Queue Storage
This service is used to handle asynchronous background tasks. When an order is placed, a message is sent to an order processing queue. The queue stores these messages until a background worker retrieves them. This separates the fast user interface from slower, heavier background work.

Azure File Share
This service acts as a fully managed cloud-based network file share. The application writes log entries to a text file stored in this file share, ensuring that application logs are stored persistently in the cloud and are accessible from anywhere.
