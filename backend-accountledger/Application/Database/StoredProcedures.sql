USE accountledger;
DELIMITER //

-- SP 1: Get Persons Paged with Search (Returns 2 result sets)
CREATE PROCEDURE sp_GetPersonsPaged(
    IN p_page INT,
    IN p_page_size INT,
    IN p_search_type VARCHAR(20),
    IN p_search_term VARCHAR(100)
)
BEGIN
    DECLARE v_offset INT;
    SET v_offset = (p_page - 1) * p_page_size;
    
    -- Result Set 1: Persons
    WITH FilteredIds AS (
        SELECT DISTINCT p.id
        FROM persons p
        LEFT JOIN accounts a ON p.id = a.person_id
        WHERE 
            p_search_type IS NULL OR 
            p_search_type = '' OR
            (p_search_type = 'IdNumber' AND p.id_number LIKE CONCAT('%', p_search_term, '%')) OR
            (p_search_type = 'Surname' AND p.surname LIKE CONCAT('%', p_search_term, '%')) OR
            (p_search_type = 'AccountNumber' AND a.account_number LIKE CONCAT('%', p_search_term, '%'))
    )
    SELECT p.id, p.id_number, p.first_name, p.surname, p.contact_info, p.created_date
    FROM FilteredIds fi
    JOIN persons p ON fi.id = p.id
    ORDER BY p.id
    LIMIT p_page_size OFFSET v_offset;
    
    -- Result Set 2: Total Count
    SELECT COUNT(*) FROM (
        SELECT DISTINCT p.id
        FROM persons p
        LEFT JOIN accounts a ON p.id = a.person_id
        WHERE 
            p_search_type IS NULL OR 
            p_search_type = '' OR
            (p_search_type = 'IdNumber' AND p.id_number LIKE CONCAT('%', p_search_term, '%')) OR
            (p_search_type = 'Surname' AND p.surname LIKE CONCAT('%', p_search_term, '%')) OR
            (p_search_type = 'AccountNumber' AND a.account_number LIKE CONCAT('%', p_search_term, '%'))
    ) AS total;
END //

-- SP 2: Get Person by ID
CREATE PROCEDURE sp_GetPersonById(IN p_person_id INT)
BEGIN
    SELECT id, id_number, first_name, surname, contact_info, created_date 
    FROM persons WHERE id = p_person_id;
END //

-- SP 3: Create Person
CREATE PROCEDURE sp_CreatePerson(
    IN p_id_number VARCHAR(20),
    IN p_first_name VARCHAR(50),
    IN p_surname VARCHAR(50),
    IN p_contact_info VARCHAR(100)
)
BEGIN
    INSERT INTO persons (id_number, first_name, surname, contact_info)
    VALUES (p_id_number, p_first_name, p_surname, p_contact_info);
    SELECT LAST_INSERT_ID();
END //

-- SP 4: Update Person
CREATE PROCEDURE sp_UpdatePerson(
    IN p_id INT,
    IN p_first_name VARCHAR(50),
    IN p_surname VARCHAR(50),
    IN p_contact_info VARCHAR(100)
)
BEGIN
    UPDATE persons 
    SET first_name = p_first_name, surname = p_surname, contact_info = p_contact_info
    WHERE id = p_id;
END //

-- SP 5: Delete Person (Rule 3: Only if no open accounts)
CREATE PROCEDURE sp_DeletePersonIfNoAccounts(IN p_person_id INT)
BEGIN
    IF EXISTS (SELECT 1 FROM accounts WHERE person_id = p_person_id AND status_id = 1) THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot delete person with open accounts. Close all accounts first.';
    END IF;
    DELETE FROM accounts WHERE person_id = p_person_id;
    DELETE FROM persons WHERE id = p_person_id;
END //

-- SP 6: Get Accounts by Person
CREATE PROCEDURE sp_GetAccountsByPersonId(IN p_person_id INT)
BEGIN
    SELECT a.id, a.person_id, a.account_number, a.outstanding_balance, 
           a.status_id, s.name AS status_name, 
           CASE WHEN s.name = 'Closed' THEN 1 ELSE 0 END AS is_closed, a.opened_date
    FROM accounts a
    JOIN account_status s ON a.status_id = s.id
    WHERE a.person_id = p_person_id
    ORDER BY a.opened_date DESC;
END //

-- SP 7: Get Account by ID
CREATE PROCEDURE sp_GetAccountById(IN p_account_id INT)
BEGIN
    SELECT a.id, a.person_id, a.account_number, a.outstanding_balance, 
           a.status_id, s.name AS status_name, 
           CASE WHEN s.name = 'Closed' THEN 1 ELSE 0 END AS is_closed, a.opened_date
    FROM accounts a
    JOIN account_status s ON a.status_id = s.id
    WHERE a.id = p_account_id;
END //

-- SP 8: Create Account
CREATE PROCEDURE sp_CreateAccount(
    IN p_person_id INT,
    IN p_account_number VARCHAR(20)
)
BEGIN
    INSERT INTO accounts (person_id, account_number, status_id)
    VALUES (p_person_id, p_account_number, 1);
    SELECT LAST_INSERT_ID();
END //

-- SP 9: Update Account
CREATE PROCEDURE sp_UpdateAccount(
    IN p_id INT,
    IN p_account_number VARCHAR(20)
)
BEGIN
    UPDATE accounts SET account_number = p_account_number WHERE id = p_id;
END //

-- SP 10: Toggle Account Status (Rule 9: Block close if balance != 0)
CREATE PROCEDURE sp_ToggleAccountStatus(
    IN p_account_id INT,
    IN p_is_open BOOLEAN
)
BEGIN
    DECLARE v_new_status_id INT;
    DECLARE v_balance DECIMAL(18,2);
    
    SET v_new_status_id = CASE WHEN p_is_open = TRUE THEN 1 ELSE 2 END;
    SELECT outstanding_balance INTO v_balance FROM accounts WHERE id = p_account_id;
    
    IF v_new_status_id = 2 AND v_balance <> 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot close account. Outstanding balance must be zero.';
    END IF;
    
    UPDATE accounts SET status_id = v_new_status_id WHERE id = p_account_id;
END //

-- SP 11: Get Transactions by Account
CREATE PROCEDURE sp_GetTransactionsByAccountId(IN p_account_id INT)
BEGIN
    SELECT id, account_id, transaction_date, amount, type, capture_date, modified_date
    FROM transactions WHERE account_id = p_account_id
    ORDER BY transaction_date DESC, capture_date DESC;
END //

-- SP 12: Get Transaction by ID
CREATE PROCEDURE sp_GetTransactionById(IN p_transaction_id INT)
BEGIN
    SELECT id, account_id, transaction_date, amount, type, capture_date, modified_date
    FROM transactions WHERE id = p_transaction_id;
END //

-- SP 13: Create Transaction (Rules 8, 10, 11)
CREATE PROCEDURE sp_CreateTransaction(
    IN p_account_id INT,
    IN p_transaction_date DATE,
    IN p_amount DECIMAL(18,2),
    IN p_type VARCHAR(10)
)
BEGIN
    DECLARE v_status_id INT;
    
    -- Rule 10: Block on closed accounts
    SELECT status_id INTO v_status_id FROM accounts WHERE id = p_account_id;
    IF v_status_id = 2 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot post transactions to a closed account.';
    END IF;
    
    -- Rule 11: Future date check
    IF p_transaction_date > CURDATE() THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Transaction date cannot be in the future.';
    END IF;
    
    START TRANSACTION;
    
    INSERT INTO transactions (account_id, transaction_date, amount, type, capture_date, modified_date)
    VALUES (p_account_id, p_transaction_date, p_amount, p_type, NOW(), NOW());
    
    SET @new_id = LAST_INSERT_ID();
    
    -- Rule 8: Atomic balance recalculation
    UPDATE accounts SET outstanding_balance = (
        SELECT COALESCE(SUM(CASE WHEN type = 'Credit' THEN amount ELSE -amount END), 0)
        FROM transactions WHERE account_id = p_account_id
    ) WHERE id = p_account_id;
    
    COMMIT;
    
    SELECT @new_id;
END //

-- SP 14: Update Transaction (Rules 8, 10)
CREATE PROCEDURE sp_UpdateTransaction(
    IN p_id INT,
    IN p_transaction_date DATE,
    IN p_amount DECIMAL(18,2),
    IN p_type VARCHAR(10)
)
BEGIN
    DECLARE v_account_id INT;
    DECLARE v_status_id INT;
    
    SELECT account_id INTO v_account_id FROM transactions WHERE id = p_id;
    IF v_account_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Transaction not found.';
    END IF;
    
    -- Rule 10: Block modification on closed accounts
    SELECT status_id INTO v_status_id FROM accounts WHERE id = v_account_id;
    IF v_status_id = 2 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot modify transactions on a closed account.';
    END IF;
    
    -- Rule 11
    IF p_transaction_date > CURDATE() THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Transaction date cannot be in the future.';
    END IF;
    
    START TRANSACTION;
    
    UPDATE transactions 
    SET transaction_date = p_transaction_date, amount = p_amount, type = p_type, modified_date = NOW()
    WHERE id = p_id;
    
    -- Rule 8: Recalculate balance
    UPDATE accounts SET outstanding_balance = (
        SELECT COALESCE(SUM(CASE WHEN type = 'Credit' THEN amount ELSE -amount END), 0)
        FROM transactions WHERE account_id = v_account_id
    ) WHERE id = v_account_id;
    
    COMMIT;
END //

DELIMITER ;