Project Overview
ABC Retail is a web application designed to demonstrate the integration of the four core Azure Storage services. It allows users to manage products and customers, upload images securely, process orders asynchronously using queues, and store persistent logs in the cloud.

2. The Four Azure Services Used
Azure Table Storage (For Structured Data)
Used to store customer profiles and product details. Data is saved in two tables: Customers and Products.

Azure Blob Storage (For Images)
Used to host product images. Images are stored in a private container named retailmedia. To keep the container secure while allowing users to view images, the application generates temporary security tokens for each image.

Azure Queue Storage (For Background Tasks)
Used to handle order and inventory processing in the background. When a user places an order, a message is sent to a queue. The user sees an immediate success message, while the actual processing happens later.

Azure File Share (For Persistent Logs)
Used to store application log files. Every order placed appends a new entry to a text file stored in the cloud. These logs can be viewed directly from a page on the website.

3. Application Features
Product Management
Users can add, view, edit, and delete products. Each product stores a name, category, price, description, and an associated image.

Customer Management
Admins can register new customers, view a full list of existing customers, and remove customers when necessary.

Secure Image Gallery
The Blob Manager page allows users to upload images, view them in a gallery, download them, or delete them permanently.

Order Processing
The Product Catalog includes a "Queue Order" button. Clicking this button sends the order to an Azure Queue for background processing, keeping the user interface fast and responsive.

Live Log Viewer
A dedicated View Logs page fetches the contents of the cloud log file and displays it on the screen for easy system monitoring.

4. How the System Flows
A typical user interaction follows this simple path:

An admin adds a new product and uploads an image.

The image goes to Blob Storage, and the product details go to Table Storage.

A customer visits the product catalog and clicks "Queue Order".

The system sends a message to the Azure Queue and shows a success message to the user.

The system writes a log entry to the Azure File Share.

The admin can view the new log entry on the View Logs page.

This setup keeps the main website fast while the cloud handles the heavy work in the background.

5. Testing Confirmation
The application was tested manually and confirmed working for all required features:

Images upload successfully to Azure Blob Storage.

Products save correctly to Azure Table Storage.

Order messages appear in the Azure Queue.

Log entries write successfully to the Azure File Share.

Users can download and delete files from the interface.

6. Required Screenshots for Submission
The following screenshots have been collected to verify the project:

From Azure Portal:

General overview of the Storage Account.

The Customers and Products tables.

The retailmedia blob container.

The order-processing queue.

The application-logs file share.

The connection strings (access keys).

From the Running Application:

The Blob Manager page with the upload form.

An uploaded image displayed in the gallery.

The Product Catalog showing items from the database.

The green success message after placing a queue order.

The View Logs page showing text pulled from Azure.

A screenshot of the Azure Portal showing an active message in the order queue.
