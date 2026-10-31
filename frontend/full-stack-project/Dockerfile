
# Stage 1: Build Angular
FROM node:22-alpine AS build

WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm ci

# Copy application source
COPY . .

# Build production application
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine AS final

# Remove default Nginx website
RUN rm -rf /usr/share/nginx/html/*

# Copy Angular production output
COPY --from=build /app/dist/full-stack-project/browser /usr/share/nginx/html

# Copy custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
