import apiRequest from '../ApiRequest.js'

const getMovies = async() => {
    const response = await apiRequest('/Movie/ReadAll', 'GET')
    if (response.ok) {
        const result = await response.json()
        return result
    }
    return null
}

export default getMovies;