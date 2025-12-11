
-- Create tables in correct order (respecting foreign key dependencies)

-- Role table
CREATE TABLE role (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL
);

-- User table
CREATE TABLE "user" (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    role_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    UNIQUE(email),
    FOREIGN KEY (role_id) REFERENCES role(role_id)
);

-- Origami table
CREATE TABLE origami (
    origami_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    image_url VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER NOT NULL,
    FOREIGN KEY (created_by) REFERENCES "user"(user_id)
);

-- Category table
CREATE TABLE category (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(100)
);

-- Guide table
CREATE TABLE guide (
    guide_id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2),
    author_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    origami_id INTEGER,
    FOREIGN KEY (author_id) REFERENCES "user"(user_id),
    FOREIGN KEY (origami_id) REFERENCES origami(origami_id) ON DELETE SET NULL
);

-- GuideCategory table (many-to-many relationship)
CREATE TABLE guide_category (
    guide_id INTEGER NOT NULL,
    category_id INTEGER NOT NULL,
    PRIMARY KEY (guide_id, category_id),
    FOREIGN KEY (guide_id) REFERENCES guide(guide_id),
    FOREIGN KEY (category_id) REFERENCES category(category_id)
);

-- Step table
CREATE TABLE step (
    step_id SERIAL PRIMARY KEY,
    guide_id INTEGER NOT NULL,
    step_number INTEGER NOT NULL,
    title VARCHAR(255),
    description TEXT,
    image_url VARCHAR(255),
    video_url VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    FOREIGN KEY (guide_id) REFERENCES guide(guide_id)
);

-- GuideAccess table
CREATE TABLE guide_access (
    access_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    guide_id INTEGER NOT NULL,
    granted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_user_guide UNIQUE (user_id, guide_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    FOREIGN KEY (guide_id) REFERENCES guide(guide_id)
);

-- Comment table
CREATE TABLE comment (
    comment_id SERIAL PRIMARY KEY,
    guide_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    content TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    parent_id INTEGER,
    FOREIGN KEY (guide_id) REFERENCES guide(guide_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    FOREIGN KEY (parent_id) REFERENCES comment(comment_id)
);

-- Favorite table
CREATE TABLE favorite (
    favorite_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    guide_id INTEGER NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    FOREIGN KEY (guide_id) REFERENCES guide(guide_id)
);

-- Course table
CREATE TABLE course (
    course_id SERIAL PRIMARY KEY,
    title VARCHAR(255),
    description TEXT,
    price DECIMAL(10, 2),
    teacher_id INTEGER,
    FOREIGN KEY (teacher_id) REFERENCES "user"(user_id)
);

-- Lesson table
CREATE TABLE lesson (
    lesson_id SERIAL PRIMARY KEY,
    course_id INTEGER,
    title VARCHAR(255),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (course_id) REFERENCES course(course_id)
);

-- Course_access table
CREATE TABLE course_access (
    access_id SERIAL PRIMARY KEY,
    course_id INTEGER,
    learner_id INTEGER,
    purchased_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (course_id) REFERENCES course(course_id),
    FOREIGN KEY (learner_id) REFERENCES "user"(user_id)
);

-- Course_review table
CREATE TABLE course_review (
    review_id SERIAL PRIMARY KEY,
    course_id INTEGER,
    user_id INTEGER,
    rating INTEGER,
    comment TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (course_id) REFERENCES course(course_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    CHECK (rating >= 1 AND rating <= 5)
);

-- Question table
CREATE TABLE question (
    question_id SERIAL PRIMARY KEY,
    course_id INTEGER,
    user_id INTEGER,
    content TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (course_id) REFERENCES course(course_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id)
);

-- Answer table
CREATE TABLE answer (
    answer_id SERIAL PRIMARY KEY,
    question_id INTEGER,
    user_id INTEGER,
    content TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (question_id) REFERENCES question(question_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id)
);

-- Challenge table
CREATE TABLE challenge (
    challenge_id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    challenge_type VARCHAR(100),
    start_date TIMESTAMP,
    end_date TIMESTAMP,
    max_team_size INTEGER,
    is_team_based BOOLEAN NOT NULL DEFAULT TRUE,
    created_by INTEGER,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (created_by) REFERENCES "user"(user_id) ON DELETE SET NULL
);

-- Team table
CREATE TABLE team (
    team_id SERIAL PRIMARY KEY,
    challenge_id INTEGER NOT NULL,
    team_name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (challenge_id) REFERENCES challenge(challenge_id) ON DELETE CASCADE
);

-- TeamMember table
CREATE TABLE team_member (
    team_member_id SERIAL PRIMARY KEY,
    team_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_team_member UNIQUE (team_id, user_id),
    FOREIGN KEY (team_id) REFERENCES team(team_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id) ON DELETE CASCADE
);

-- Submission table
CREATE TABLE submission (
    submission_id SERIAL PRIMARY KEY,
    challenge_id INTEGER NOT NULL,
    team_id INTEGER,
    submitted_by INTEGER NOT NULL,
    file_url VARCHAR(500),
    submitted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (challenge_id) REFERENCES challenge(challenge_id) ON DELETE CASCADE,
    FOREIGN KEY (team_id) REFERENCES team(team_id),
    FOREIGN KEY (submitted_by) REFERENCES "user"(user_id) ON DELETE CASCADE
);

-- Score table
CREATE TABLE score (
    score_id SERIAL PRIMARY KEY,
    submission_id INTEGER NOT NULL,
    score DECIMAL(5, 2) NOT NULL,
    score_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    score_by INTEGER NOT NULL,
    FOREIGN KEY (submission_id) REFERENCES submission(submission_id),
    FOREIGN KEY (score_by) REFERENCES "user"(user_id)
);

-- Vote table
CREATE TABLE vote (
    vote_id SERIAL PRIMARY KEY,
    submission_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    voted_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_vote UNIQUE (submission_id, user_id),
    FOREIGN KEY (submission_id) REFERENCES submission(submission_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id)
);

-- Leaderboard table
CREATE TABLE leaderboard (
    leaderboard_id SERIAL PRIMARY KEY,
    challenge_id INTEGER NOT NULL,
    team_id INTEGER,
    user_id INTEGER,
    total_score DECIMAL(10, 2) NOT NULL DEFAULT 0,
    rank INTEGER,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (challenge_id) REFERENCES challenge(challenge_id) ON DELETE CASCADE,
    FOREIGN KEY (team_id) REFERENCES team(team_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    CONSTRAINT ck_leaderboard_team_or_user CHECK (
        (team_id IS NOT NULL AND user_id IS NULL) OR 
        (team_id IS NULL AND user_id IS NOT NULL)
    )
);

-- Badge table
CREATE TABLE badge (
    badge_id SERIAL PRIMARY KEY,
    badge_name VARCHAR(100) NOT NULL,
    badge_description VARCHAR(255),
    condition_type VARCHAR(50),
    condition_value VARCHAR(50)
);

-- User_badge table
CREATE TABLE user_badge (
    user_id INTEGER NOT NULL,
    badge_id INTEGER NOT NULL,
    earned_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, badge_id),
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    FOREIGN KEY (badge_id) REFERENCES badge(badge_id)
);

-- Wallet table
CREATE TABLE wallet (
    wallet_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    balance DECIMAL(18, 2) DEFAULT 0,
    updated_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id)
);

-- Orders table
CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    buyer_user_id INTEGER NOT NULL,
    total_amount DECIMAL(18, 2) NOT NULL,
    currency VARCHAR(10) NOT NULL DEFAULT 'VND',
    status VARCHAR(20) NOT NULL DEFAULT 'pending',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    FOREIGN KEY (buyer_user_id) REFERENCES "user"(user_id)
);

-- OrderItem table
CREATE TABLE order_item (
    order_item_id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL,
    product_type VARCHAR(20) NOT NULL,
    product_id INTEGER NOT NULL,
    unit_price DECIMAL(18, 2) NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 1,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    CHECK (product_type = 'course' OR product_type = 'guide')
);

-- Transaction table
CREATE TABLE transaction (
    transaction_id SERIAL PRIMARY KEY,
    sender_wallet_id INTEGER,
    receiver_wallet_id INTEGER,
    amount DECIMAL(18, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    transaction_type VARCHAR(30),
    status VARCHAR(20),
    order_id INTEGER,
    FOREIGN KEY (sender_wallet_id) REFERENCES wallet(wallet_id),
    FOREIGN KEY (receiver_wallet_id) REFERENCES wallet(wallet_id),
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    CHECK (transaction_type IN ('deposit', 'purchase', 'refund', 'transfer'))
);

-- Revenue table
CREATE TABLE revenue (
    revenue_id SERIAL PRIMARY KEY,
    user_id INTEGER,
    course_id INTEGER,
    guide_id INTEGER,
    amount DECIMAL(10, 2),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    FOREIGN KEY (course_id) REFERENCES course(course_id),
    FOREIGN KEY (guide_id) REFERENCES guide(guide_id)
);

-- Commission table
CREATE TABLE commission (
    commission_id SERIAL PRIMARY KEY,
    revenue_id INTEGER,
    percent DECIMAL(5, 2),
    FOREIGN KEY (revenue_id) REFERENCES revenue(revenue_id)
);

-- Ticket_type table
CREATE TABLE ticket_type (
    ticket_type_id SERIAL PRIMARY KEY,
    ticket_type_name VARCHAR(50)
);

-- Ticket table
CREATE TABLE ticket (
    ticket_id SERIAL PRIMARY KEY,
    user_id INTEGER,
    ticket_type_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20),
    title VARCHAR(255),
    content TEXT,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id),
    FOREIGN KEY (ticket_type_id) REFERENCES ticket_type(ticket_type_id)
);

-- Notification table
CREATE TABLE notification (
    notification_id SERIAL PRIMARY KEY,
    user_id INTEGER,
    content TEXT,
    is_read BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id)
);

-- Refresh_token table
CREATE TABLE refresh_token (
    token_id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL,
    refresh_token VARCHAR(512) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP,
    is_active BOOLEAN NOT NULL,
    FOREIGN KEY (user_id) REFERENCES "user"(user_id)
);

-- Create indexes for better performance (matching SQL Server indexes)
CREATE INDEX idx_orders_buyer ON orders(buyer_user_id);
CREATE INDEX idx_transaction_order_id ON transaction(order_id);

-- Additional useful indexes for common queries
CREATE INDEX idx_guide_author ON guide(author_id);
CREATE INDEX idx_guide_origami ON guide(origami_id);
CREATE INDEX idx_step_guide ON step(guide_id);
CREATE INDEX idx_comment_guide ON comment(guide_id);
CREATE INDEX idx_comment_user ON comment(user_id);
CREATE INDEX idx_favorite_user ON favorite(user_id);
CREATE INDEX idx_favorite_guide ON favorite(guide_id);
CREATE INDEX idx_course_teacher ON course(teacher_id);
CREATE INDEX idx_lesson_course ON lesson(course_id);
CREATE INDEX idx_leaderboard_challenge ON leaderboard(challenge_id);
CREATE INDEX idx_submission_challenge ON submission(challenge_id);
CREATE INDEX idx_team_challenge ON team(challenge_id);
CREATE INDEX idx_transaction_sender ON transaction(sender_wallet_id);
CREATE INDEX idx_transaction_receiver ON transaction(receiver_wallet_id);
CREATE INDEX idx_wallet_user ON wallet(user_id);
CREATE INDEX idx_refresh_token_user ON refresh_token(user_id);

ALTER TABLE answer RENAME TO "Answer";
ALTER TABLE badge RENAME TO "Badge";
ALTER TABLE category RENAME TO "Category";
ALTER TABLE challenge RENAME TO "Challenge";
ALTER TABLE comment RENAME TO "Comment";
ALTER TABLE commission RENAME TO "Commission";
ALTER TABLE course RENAME TO "Course";
ALTER TABLE course_access RENAME TO "Course_access";
ALTER TABLE course_review RENAME TO "Course_review";
ALTER TABLE favorite RENAME TO "Favorite";
ALTER TABLE guide RENAME TO "Guide";
ALTER TABLE guide_access RENAME TO "GuideAccess";
ALTER TABLE guide_category RENAME TO "GuideCategory";
ALTER TABLE leaderboard RENAME TO "Leaderboard";
ALTER TABLE lesson RENAME TO "Lesson";
ALTER TABLE notification RENAME TO "Notification";
ALTER TABLE order_item RENAME TO "OrderItem";
ALTER TABLE orders RENAME TO "Orders";
ALTER TABLE origami RENAME TO "Origami";
ALTER TABLE question RENAME TO "Question";
ALTER TABLE refresh_token RENAME TO "Refresh_token";
ALTER TABLE revenue RENAME TO "Revenue";

ALTER TABLE score RENAME TO "Score";
ALTER TABLE step RENAME TO "Step";
ALTER TABLE submission RENAME TO "Submission";
ALTER TABLE team RENAME TO "Team";
ALTER TABLE team_member RENAME TO "TeamMember";
ALTER TABLE ticket RENAME TO "Ticket";
ALTER TABLE ticket_type RENAME TO "Ticket_type";
ALTER TABLE transaction RENAME TO "Transaction";
ALTER TABLE "user" RENAME TO "User";        -- user là từ khóa, cần ngoặc kép
ALTER TABLE user_badge RENAME TO "User_badge";
ALTER TABLE vote RENAME TO "Vote";
ALTER TABLE wallet RENAME TO "Wallet";
