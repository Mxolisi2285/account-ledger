-- Replace the existing fn_get_person_by_id function with this corrected version:
CREATE OR REPLACE FUNCTION fn_get_person_by_id(p_id INTEGER)
RETURNS TABLE (
    id INTEGER,
    id_number VARCHAR,
    first_name VARCHAR,
    surname VARCHAR,
    contact_info VARCHAR,
    created_date TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        p.id,          
        p.id_number,
        p.first_name,
        p.surname,
        p.contact_info,
        p.created_date
    FROM persons p     
    WHERE p.id = p_id; 
END;
$$ LANGUAGE plpgsql;