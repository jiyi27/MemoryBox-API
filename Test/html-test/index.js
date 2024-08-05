/** @namespace data.token **/

const API_URL = 'http://localhost:5190'
let token = ''

const registerForm = document.getElementById('registerForm')
const loginForm = document.getElementById('loginForm')
const createPostForm = document.getElementById('createPostForm')
const getBoxesButton = document.getElementById('getBoxesBtn')
const getPostsButton = document.getElementById('getPostsBtn')
const deletePostButton = document.getElementById('deletePostBtn')
const deleteBoxButton = document.getElementById('deleteBoxBtn')

registerForm.addEventListener('submit', async (e) => {
    // Submitting form would refresh the page, so we prevent page reload.
    e.preventDefault();

    const fullName = registerForm.regFullName.value;
    const username = registerForm.regUsername.value;
    const password = registerForm.regPassword.value;
    const email = registerForm.regEmail.value;
    const fileInput = document.getElementById('profileImage')[0];

    const formData = new FormData();
    formData.append('fullName', fullName);
    formData.append('username', username);
    formData.append('password', password);

    if (email) {
        formData.append('email', email);
    }
    if (fileInput) {
        formData.append('profileImage', fileInput);
    }

    try {
        const response = await axios.post(`${API_URL}/api/auth/register`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });

        console.log("User registered successfully");
    } catch (error) {
        handleAxiosError(error, 'register');
    }
});

loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const formData = {
        username: loginForm.loginUsername.value,
        password: loginForm.loginPassword.value
    };

    try {
        const response = await axios.post(`${API_URL}/api/auth/login`, formData, {
            headers: {'Content-Type': 'application/json'}
        });

        // No need to parse JSON, axios does it for us.
        // No need to check status code, axios throws error if status code is not 2xx.
        token = data.token;
    } catch (error) {
        handleAxiosError(error, 'login');
    }
});

createPostForm.addEventListener('submit', async (e) => {
    e.preventDefault();

    const title = createPostForm.postTitle.value;
    const content = createPostForm.postContent.value;
    const imageFiles = document.getElementById('postImages').files;
    const boxId = document.getElementById('boxId').value;

    const formData = new FormData();
    if (title) {
        formData.append('title', title);
    }
    if (content) {
        formData.append('content', content);
    }
    for (let i = 0; i < imageFiles.length; i++) {
        formData.append('images', imageFiles[i]);
    }

    try {
        await axios.post(`${API_URL}/api/boxes/${boxId}/posts`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
        createPostForm.reset();
        console.log("Post created successfully");
    } catch (error) {
        handleAxiosError(error, 'create post');
    }
});

getBoxesButton.addEventListener('click', async () => {
    try {
        const response = await axios.get(`${API_URL}/api/boxes`);
        console.log(response.data);
    } catch (error) {
        handleAxiosError(error, 'get boxes');
    }
})

getPostsButton.addEventListener('click', async () => {
    const boxId = document.getElementById('boxId').value;
    try {
        const response = await axios.get(`${API_URL}/api/boxes/${boxId}/posts`);
        console.log(response.data);
    } catch (error) {
        handleAxiosError(error, 'get posts');
    }
})

function handleAxiosError(error, action) {
    if (error.response) {
        console.log(`Failed to ${action}, status code:`, error.response.status);
        console.log("Error data:", error.response.data);
    } else {
        console.log(`Failed to ${action}:`, error.message);
    }
}
