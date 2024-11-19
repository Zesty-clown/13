import secrets
import sqlite3
import os

def create_connection(db_file):
    """Create a database connection to a SQLite database."""
    conn = None
    try:
        conn = sqlite3.connect(db_file)
        print(f"Connected to SQLite database: {db_file}")
    except sqlite3.Error as e:
        print(f"Error connecting to database: {e}")
    return conn

def create_table(conn):
    """Create a table to store tokens."""
    try:
        sql_create_tokens_table = """CREATE TABLE IF NOT EXISTS tokens (
                                        id INTEGER PRIMARY KEY,
                                        token TEXT NOT NULL
                                    );"""
        c = conn.cursor()
        c.execute(sql_create_tokens_table)
    except sqlite3.Error as e:
        print(f"Error creating table: {e}")

def save_token_to_db(conn, token):
    """Save a token to the database."""
    try:
        sql_insert_token = """INSERT INTO tokens(token) VALUES(?);"""
        c = conn.cursor()
        c.execute(sql_insert_token, (token,))
        conn.commit()
        print("Token saved to database.")
    except sqlite3.Error as e:
        print(f"Error saving token to database: {e}")

def generate_token():
    token = secrets.token_hex(16)  # Generate a secure token
    return token

def main():
    database = os.path.join(os.path.join(os.environ['USERPROFILE']), 'Desktop', 'auth_tokens.db')
    
    # Create a database connection
    conn = create_connection(database)
    
    # Create table
    if conn is not None:
        create_table(conn)
        
        # Generate a token
        token = generate_token()
        print(f"Generated Token: {token}")
        
        # Save token to the database
        save_token_to_db(conn, token)
        
        # Close the connection
        conn.close()
    else:
        print("Error! Cannot create the database connection.")

if __name__ == "__main__":
    main()
