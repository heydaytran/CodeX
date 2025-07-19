CREATE SCHEMA IF NOT EXISTS customer;


 
-- customer table (main customer record)
CREATE TABLE  IF NOT EXISTS customer.customers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique customer ID
    tenant_id UUID REFERENCES common.tenants(id) ON DELETE CASCADE, -- Link to tenant
    user_id UUID NOT NULL, -- Optional: Link to user account 
    title VARCHAR(10), -- e.g. Mr, Mrs, Ms, Dr
    first_name VARCHAR(100) , -- Customer's first name
    last_name VARCHAR(100) , -- Customer's last name
    date_of_birth DATE , -- DOB for age-related pricing/insurance
    gender VARCHAR(20), -- Optional gender field
    created_at TIMESTAMP  DEFAULT CURRENT_TIMESTAMP, -- When the record was created
    updated_at TIMESTAMP  DEFAULT CURRENT_TIMESTAMP, -- When the record was last updated
    deleted_at TIMESTAMP -- When the record was deleted (soft delete)
);
 
 
-- contract informations table (Stores contact details for customers)
CREATE TABLE  IF NOT EXISTS customer.contact_informations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique ID for contact info
    customer_id UUID REFERENCES customer.customers(id) ON DELETE CASCADE, -- Link to customer
    email VARCHAR(255) , -- Primary contact email
    phone_number VARCHAR(30) , -- Main telephone number
    mobile_number VARCHAR(30), -- Optional mobile number
    address_line1 VARCHAR(255) , -- Address line 1
    address_line2 VARCHAR(255), -- Address line 2
    city VARCHAR(100) , -- City or town
    postcode VARCHAR(20) , -- Postcode/ZIP
    county VARCHAR(100), -- County or region
    country VARCHAR(100),  -- Country name
    created_at TIMESTAMP  DEFAULT CURRENT_TIMESTAMP, -- When the record was created
    updated_at TIMESTAMP  DEFAULT CURRENT_TIMESTAMP, -- When the record was last updated
    deleted_at TIMESTAMP -- When the record was deleted (soft delete)
);
 
-- create the customer_type_enum and contact_method_enum types if they do not exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'customer_type_enum') THEN
        CREATE TYPE customer_type_enum AS ENUM ('Retail', 'Agent', 'Trade');
    END IF;
 
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'contact_method_enum') THEN
        CREATE TYPE contact_method_enum AS ENUM ('Email', 'Phone', 'Post');
    END IF;
END
$$;
 
-- customer preferences and profiles table (Stores customer preferences)
CREATE TABLE IF NOT EXISTS customer.preferences_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique ID for preferences
    customer_id UUID REFERENCES customer.customers(id) ON DELETE CASCADE, -- Link to customer
    customer_type customer_type_enum , -- 'Retail', 'Agent', 'Trade', etc.
    preferred_departure_region VARCHAR(100), -- E.g. "North West" or FK to regions table
    preferred_contact_method contact_method_enum, -- 'Email', 'Phone', 'Post'
    preferred_contact_time VARCHAR(50), -- E.g. "Evenings", "Anytime"
    language_preference VARCHAR(50), -- E.g. "en-GB", "fr-FR"
    travel_interests TEXT, -- E.g. "Cruises, City Breaks" (or use tags)
    notes TEXT -- Internal notes about the customer
);
 
-- customer bookings & Loyalty table (Stores booking history)
CREATE TABLE IF NOT EXISTS customer.booking_loyalties (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique ID for booking loyalty record
    customer_id UUID REFERENCES customer.customers(id) ON DELETE CASCADE, -- Link to customer
    first_booking_date DATE, -- Date of their first booking
    last_booking_date DATE, -- Date of most recent booking
    booking_count INT  DEFAULT 0, -- Total bookings made
    total_spent DECIMAL(10,2)  DEFAULT 0.00, -- Total revenue from this customer
    loyalty_points_balance INT  DEFAULT 0 -- Points for rewards, if applicable
);
 
-- customer marketing compliances table ( Customer consent (or refusal) for marketing activities and GDPR compliance)
CREATE TABLE IF NOT EXISTS customer.marketing_compliances (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(), -- Unique ID for compliance record
    customer_id UUID REFERENCES customer.customers(id) ON DELETE CASCADE, -- Link to customer
    marketing_opt_in BOOLEAN, -- Overall consent to receive marketing
    email_opt_in BOOLEAN, -- Consent to email campaigns
    sms_opt_in BOOLEAN, -- Consent to SMS messages
    gdpr_consent_date DATE, -- Date consent was last given
    do_not_contact BOOLEAN -- Force suppress all marketing contact
);
 
-- indexers for performance
CREATE INDEX IF NOT EXISTS idx_customers_email ON customer.contact_informations(email);