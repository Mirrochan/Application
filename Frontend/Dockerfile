### Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files and install deps
COPY package.json package-lock.json* ./
RUN npm ci --silent

# Copy source and build
COPY . .
RUN npm run build -- --configuration=production

### Production stage (nginx)
FROM nginx:stable-alpine AS runtime
COPY --from=build /app/dist/frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
