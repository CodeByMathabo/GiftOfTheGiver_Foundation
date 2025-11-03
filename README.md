# **Project Name:** GiftOfTheGiver_Foundation

## **Application Name:** Mofani *(a South Sesotho word, meaning 'Giver')*
------------------------------------------------------------------------
### **How to run the application:**

* **Step 1:** Copy then App URL Link
  
* **Step 2:** Open your web browser
  
* **Step 3:** Paste the URL Link in the search bar
  
* **Step 4:** Click enter and the app will run

* **Step 5:** If you want to use the app as a Donor or a Volunteer, you must register your account, select your role, and finally login 
----------------------------------------------------------------------
### **App URL:**
* https://mofani-e6abg9fkbvhjawhu.spaincentral-01.azurewebsites.net/
----------------------------------------------------------------------
### **Important Information**
* This is a C# Web Application for the *Gift Of The Givers Foundation*

* This application has three user roles and uses role based access control

* **The user roles are:** Admin, Donor, Volunteer

* Each user role has access to specific pages

* Admins can promte and demote each other

* An admin cannot demote their own account, a fellow admin must demote their account 
-----------------------------------------------------------------------
### **Access guideline for the Administrator:**
* **Important:** You cannot register an admin. Admins are grated access to the system by other existing admins.
  
* Only existing admins have the ability to promote volunteers to become fellow admins in the Team page
  
* Only exisitng admins have the ability to demote fellow admins in the Team page
  
* **Fact:** When the app was ran for the first time, admin credentials where entered in the backend, and that is how the first admin user role was created

-----------------------------------------------------------------------
### **How to Promote an Admin user role:**
* **Step 1:** Register as a volunteer
  
* **Step 2:** In the Team page, an existing admin will complete a veryfication form to approve and link your volunteer account to allow you to be promoted to the Admin user role later
  
* **Step 3:** Once your account is linked, it will appear under the User Administrators table as a potential admin in the Team page.
  
* **Step 4:** Now that your volunteer account appear under the User Administrators table, admins can promote you to the admin user role and your account can be demoted
  
* **Keep in mind:** *Only admins have access to the Team page*
-----------------------------------------------------------------------
### **Notable Technical Tools used to build this system:**
* **Role Based Access Contol:** Used to give specific users access to specific pages

* **Azure DevOps:** Used for project planning
  
* **Visual Studio 2022:** Used as an Editor
  
* **LocalDb:** Used as development database
  
* **bUnit:** A package for Unit Testing
  
* **Moq:** A package to simulate as a mock database for Unit Testing
  
* **Microsoft.EntityFrameworkCore.InMemory:** In-memory database provider
  
* **Azure Repository:** Used for version control
  
* **GitHub Repository:** Used for version control
  
* **Azure Pipeline:** Used for CI/CD
  
* **Azure SQL Database:** Used as production database
  
* **Azure App Service:** Used to contain and deploy the web application

