CREATE TABLE Users (
    Id NVARCHAR(450) PRIMARY KEY,  -- From Identity
    UserName NVARCHAR(256),
    Email NVARCHAR(256),
    PhoneNumber NVARCHAR(50),
    Title NVARCHAR(100),          -- E.g., "Software Engineer"
    Bio NVARCHAR(MAX),            -- User bio/profile summary
    Address NVARCHAR(200),
    Location NVARCHAR(100),       -- User's preferred job location
    CareerLevel NVARCHAR(50),     -- Junior, Mid, Senior, etc.
    ExperienceYears INT,
    SubscriptionPlanId INT,       -- Foreign key to SubscriptionPlans
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE Roles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256)            -- Admin, JobPoster, JobApplicant
);
CREATE TABLE SubscriptionPlans (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50),            -- E.g., "Basic", "Premium"
    MonthlyApplicationLimit INT,  -- E.g., 10 for free, 50 for premium
    Price DECIMAL(10, 2),
    StripePlanId NVARCHAR(100)    -- Stripe subscription plan ID
);
CREATE TABLE Jobs (
    Id INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(100),
    Description NVARCHAR(MAX),
    Requirements NVARCHAR(MAX),
    CategoryId INT,              -- Foreign key to Categories
    CareerLevel NVARCHAR(50),    -- Internship, Junior, Senior
    JobType NVARCHAR(50),        -- Full-time, Part-time, Remote, Freelance
    Location NVARCHAR(100),
    IsClosed BIT DEFAULT 0,      -- Is job closed?
    PostedBy NVARCHAR(450),      -- UserId of the JobPoster
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    FOREIGN KEY (PostedBy) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL

);

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(50)            -- Software, Business, Marketing, etc.
);

CREATE TABLE Applications (
    Id INT PRIMARY KEY IDENTITY,
    JobId INT,
    UserId NVARCHAR(450),        -- Applicant's UserId
    YearsOfExperience INT,
    PreviousExperience NVARCHAR(MAX),
    ExpectedSalary DECIMAL(10, 2),
    Phone NVARCHAR(20),
    CVFilePath NVARCHAR(255),    -- Path to uploaded CV file
    AppliedAt DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Accepted, Rejected
    FOREIGN KEY (JobId) REFERENCES Jobs(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
CREATE TABLE Payments (
    Id INT PRIMARY KEY IDENTITY,
    UserId NVARCHAR(450),
    SubscriptionPlanId INT,
    StripePaymentIntentId NVARCHAR(100),
    Amount DECIMAL(10, 2),
    PaymentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20),         -- Succeeded, Failed, Processing
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (SubscriptionPlanId) REFERENCES SubscriptionPlans(Id)
);