-- Script để fix lỗi duplicate key cho Wallet table
-- Chạy script này trên PostgreSQL database của bạn
-- Lưu ý: Table name là "Wallet" (viết hoa, có dấu ngoặc kép) trên Render

-- 1. Thêm unique constraint trên user_id để đảm bảo mỗi user chỉ có 1 wallet
-- (Bỏ qua nếu constraint đã tồn tại)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'UQ__Wallet__user_id' 
        AND conrelid = '"Wallet"'::regclass
    ) THEN
        ALTER TABLE "Wallet" 
        ADD CONSTRAINT UQ__Wallet__user_id UNIQUE (user_id);
        RAISE NOTICE 'Đã thêm unique constraint trên user_id';
    ELSE
        RAISE NOTICE 'Unique constraint đã tồn tại';
    END IF;
END $$;

-- 2. Fix sequence nếu bị lệch
-- Đặt sequence value về MAX(wallet_id) + 1 để tránh duplicate key
DO $$
DECLARE
    seq_name TEXT;
    max_id INTEGER;
    new_seq_val INTEGER;
BEGIN
    -- Tìm sequence name
    seq_name := pg_get_serial_sequence('"Wallet"', 'wallet_id');
    
    IF seq_name IS NULL THEN
        RAISE EXCEPTION 'Không tìm thấy sequence cho Wallet.wallet_id';
    END IF;
    
    -- Lấy MAX wallet_id hiện tại
    SELECT COALESCE(MAX(wallet_id), 0) INTO max_id FROM "Wallet";
    
    -- Set sequence về max_id + 1
    new_seq_val := max_id + 1;
    EXECUTE format('SELECT setval(%L, %s, false)', seq_name, new_seq_val);
    
    RAISE NOTICE 'Đã set sequence % về giá trị %', seq_name, new_seq_val;
END $$;

-- 3. Kiểm tra sequence hiện tại và so sánh với MAX wallet_id
DO $$
DECLARE
    seq_name TEXT;
    seq_last_value BIGINT;
    max_wallet_id INTEGER;
BEGIN
    -- Tìm sequence name
    seq_name := pg_get_serial_sequence('"Wallet"', 'wallet_id');
    
    IF seq_name IS NULL THEN
        RAISE EXCEPTION 'Không tìm thấy sequence cho Wallet.wallet_id';
    END IF;
    
    -- Lấy last_value từ sequence (không cần currval)
    -- seq_name có dạng "schema.sequence_name", cần format đúng
    EXECUTE format('SELECT last_value FROM %s', seq_name) INTO seq_last_value;
    
    -- Lấy MAX wallet_id
    SELECT COALESCE(MAX(wallet_id), 0) INTO max_wallet_id FROM "Wallet";
    
    -- Hiển thị kết quả
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Sequence name: %', seq_name;
    RAISE NOTICE 'Sequence last_value: %', seq_last_value;
    RAISE NOTICE 'MAX wallet_id: %', max_wallet_id;
    
    IF seq_last_value <= max_wallet_id THEN
        RAISE WARNING 'WARNING: Sequence value (%) nhỏ hơn hoặc bằng MAX wallet_id (%) - có thể gây duplicate key!', seq_last_value, max_wallet_id;
    ELSE
        RAISE NOTICE 'OK: Sequence value (%) lớn hơn MAX wallet_id (%)', seq_last_value, max_wallet_id;
    END IF;
    RAISE NOTICE '========================================';
END $$;

-- 4. Query đơn giản để xem thông tin (không dùng currval)
-- Sử dụng pg_sequences view để lấy thông tin sequence
SELECT 
    pg_get_serial_sequence('"Wallet"', 'wallet_id') as sequence_name,
    s.last_value as sequence_last_value,
    (SELECT MAX(wallet_id) FROM "Wallet") as max_wallet_id,
    CASE 
        WHEN s.last_value <= COALESCE((SELECT MAX(wallet_id) FROM "Wallet"), 0)
        THEN 'WARNING: Có thể gây duplicate key!'
        ELSE 'OK'
    END as status
FROM pg_sequences s
WHERE s.sequencename = split_part(pg_get_serial_sequence('"Wallet"', 'wallet_id'), '.', 2)
  AND s.schemaname = split_part(pg_get_serial_sequence('"Wallet"', 'wallet_id'), '.', 1);

-- ========================================
-- FIX SEQUENCE CHO TRANSACTION TABLE
-- ========================================

-- 5. Fix sequence cho Transaction nếu bị lệch
DO $$
DECLARE
    seq_name TEXT;
    max_id INTEGER;
    new_seq_val INTEGER;
BEGIN
    -- Tìm sequence name
    seq_name := pg_get_serial_sequence('"Transaction"', 'transaction_id');
    
    IF seq_name IS NULL THEN
        RAISE EXCEPTION 'Không tìm thấy sequence cho Transaction.transaction_id';
    END IF;
    
    -- Lấy MAX transaction_id hiện tại
    SELECT COALESCE(MAX(transaction_id), 0) INTO max_id FROM "Transaction";
    
    -- Set sequence về max_id + 1
    new_seq_val := max_id + 1;
    EXECUTE format('SELECT setval(%L, %s, false)', seq_name, new_seq_val);
    
    RAISE NOTICE 'Đã set sequence % về giá trị %', seq_name, new_seq_val;
END $$;

-- 6. Kiểm tra sequence Transaction
SELECT 
    pg_get_serial_sequence('"Transaction"', 'transaction_id') as sequence_name,
    s.last_value as sequence_last_value,
    (SELECT MAX(transaction_id) FROM "Transaction") as max_transaction_id,
    CASE 
        WHEN s.last_value <= COALESCE((SELECT MAX(transaction_id) FROM "Transaction"), 0)
        THEN 'WARNING: Có thể gây duplicate key!'
        ELSE 'OK'
    END as status
FROM pg_sequences s
WHERE s.sequencename = split_part(pg_get_serial_sequence('"Transaction"', 'transaction_id'), '.', 2)
  AND s.schemaname = split_part(pg_get_serial_sequence('"Transaction"', 'transaction_id'), '.', 1);

