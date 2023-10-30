

v202302 = '''m_EditorVersion: 2022.2.6f1
m_EditorVersionWithRevision: 2022.2.6f1 (10bfa6201ced)
'''

v202303 = '''m_EditorVersion: 2022.3.0f1c1
m_EditorVersionWithRevision: 2022.3.0f1c1 (aad67108fc1f)
'''

current_version = '202302'


with open('ProjectSettings\\ProjectVersion.txt') as file_handle:
    content = file_handle.read().strip()

    if content == v202302.strip():
        current_version = '202302'
    else:
        current_version = '202303'


with open('ProjectSettings\\ProjectVersion.txt', 'w') as file_handle:
    if current_version == '202302':
        file_handle.write(v202303)
    else:
        file_handle.write(v202302)

print(f"Switched to version: " + current_version)
