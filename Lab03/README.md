Tricky Question:
How would you design the sorting logic to prevent injection of arbitrary property names via
query parameters? What are the risks if you don?t?
To prevent injection, I would only allow sorting by specific safe fields like "title" or "year", and if the user tries anything else, I’d default to sorting by "id"
Example I’m avoiding: a user sends ?sortBy=aadflkjdfaslkdasf (or Title;DROP TABLE Books in string-built queries), which could crash the API or be abused if sorting is built from raw strings.
