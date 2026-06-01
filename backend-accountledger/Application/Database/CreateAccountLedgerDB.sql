-- 1. CREATE DATABASE
DROP DATABASE IF EXISTS accountledger;
CREATE DATABASE accountledger CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE accountledger;

-- 2. TABLES & CONSTRAINTS
CREATE TABLE users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
) ENGINE=InnoDB;

CREATE TABLE persons (
    id INT PRIMARY KEY AUTO_INCREMENT,
    id_number VARCHAR(20) NOT NULL UNIQUE,
    first_name VARCHAR(50) NOT NULL,
    surname VARCHAR(50) NOT NULL,
    contact_info VARCHAR(100),
    created_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_id_number (id_number),
    INDEX idx_surname (surname)
) ENGINE=InnoDB;

CREATE TABLE account_status (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(20) NOT NULL UNIQUE
) ENGINE=InnoDB;

CREATE TABLE accounts (
    id INT PRIMARY KEY AUTO_INCREMENT,
    person_id INT NOT NULL,
    account_number VARCHAR(20) NOT NULL UNIQUE,
    outstanding_balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    status_id INT NOT NULL,
    opened_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (person_id) REFERENCES persons(id) ON DELETE RESTRICT,
    FOREIGN KEY (status_id) REFERENCES account_status(id),
    INDEX idx_person_id (person_id),
    INDEX idx_account_number (account_number)
) ENGINE=InnoDB;

CREATE TABLE transactions (
    id INT PRIMARY KEY AUTO_INCREMENT,
    account_id INT NOT NULL,
    transaction_date DATE NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    type ENUM('Debit', 'Credit') NOT NULL,
    capture_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE RESTRICT,
    CHECK (amount <> 0),
    INDEX idx_account_id (account_id),
    INDEX idx_transaction_date (transaction_date)
) ENGINE=InnoDB;

-- 3. SAMPLE DATA
INSERT INTO users (username, password_hash) VALUES ('admin', 'admin123');
INSERT INTO account_status (name) VALUES ('Open'), ('Closed');

INSERT INTO persons (id_number, first_name, surname, contact_info) VALUES
('8501015000089', 'John', 'Doe', 'john.doe@example.com'),
('9012315000085', 'Jane', 'Smith', 'jane.smith@example.com'),
('7505125000081', 'Robert', 'Brown', NULL);

INSERT INTO accounts (person_id, account_number, outstanding_balance, status_id) VALUES
(1, 'ACC-JD-001', 1500.00, 1),
(1, 'ACC-JD-002', 0.00, 2),
(2, 'ACC-JS-001', 0.00, 1);

INSERT INTO transactions (account_id, transaction_date, amount, type) VALUES
(1, '2024-01-15', 2000.00, 'Credit'),
(1, '2024-02-10', 500.00, 'Debit');