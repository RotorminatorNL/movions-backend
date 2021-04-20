const apiRequest = (route, method, body = undefined, headers = undefined) => fetch(route, {
    body: (body !== undefined)
        ? JSON.stringify(body)
        : undefined,
    method: method,
    headers: {
        ...((body !== undefined)
            ? {
                'Content-Type': 'application/json'
            }
            : undefined),
        ...headers
    }
})

export default apiRequest;