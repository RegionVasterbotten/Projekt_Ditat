# USAGE
# python detect_barcode.py --image images/barcode_01.jpg

# import the necessary packages
import numpy as np
import argparse
import cv2

# construct the argument parse and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", required = True, help = "path to the image file")
args = vars(ap.parse_args())

# load the image and convert it to grayscale
image = cv2.imread(args["image"])

#if (image==None):print ("Image not found." + args["image"])

#cv2.imshow('image',image)
#k = cv2.waitKey(0)



#Scaling
imgHeight, imgWidth = image.shape[:2]

scaleFactor = 1
if (imgHeight>2048): scaleFactor = 2048/imgHeight
image = cv2.resize(image, (0,0), fx=scaleFactor, fy=scaleFactor, interpolation=cv2.INTER_AREA)

gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# compute the Scharr gradient magnitude representation of the images
# in both the x and y direction
gradX = cv2.Sobel(gray, ddepth = cv2.CV_32F, dx = 1, dy = 0, ksize = -1)
gradY = cv2.Sobel(gray, ddepth = cv2.CV_32F, dx = 0, dy = 1, ksize = -1)

# subtract the y-gradient from the x-gradient
gradient = cv2.subtract(gradX, gradY)
gradient = cv2.convertScaleAbs(gradient)

# blur and threshold the image
blurred = cv2.blur(gradient, (9, 9))
(_, thresh) = cv2.threshold(blurred, 225, 255, cv2.THRESH_BINARY)

# construct a closing kernel and apply it to the thresholded image
kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (21, 7))
closed = cv2.morphologyEx(thresh, cv2.MORPH_CLOSE, kernel)

# perform a series of erosions and dilations
closed = cv2.erode(closed, None, iterations = 4)
closed = cv2.dilate(closed, None, iterations = 4)

# find the contours in the thresholded image, then sort the contours
# by their area, keeping only the largest one
(_, cnts, _) = cv2.findContours(closed.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
c = sorted(cnts, key = cv2.contourArea, reverse = True)[0]

# compute the rotated bounding box of the largest contour
rect = cv2.minAreaRect(c)
box = np.int0(cv2.boxPoints(rect))

# draw a bounding box arounded the detected barcode and display the
# image

#cv2.drawContours(image, [box], -1, (0, 255, 0), 3)
#cv2.imshow("Image", image)
#cv2.waitKey(0)

print (box[0][0], box[0][1])
print (box[1][0], box[1][1])
print (box[2][0], box[2][1])
print (box[3][0], box[3][1])

bcX, bcY, bcWidth, bcHeight = cv2.boundingRect(box)

addWidth = int(bcWidth * 0.5);
addHeight = int(bcHeight * 0.5);

bcX = bcX - addWidth;
bcWidth = bcWidth + (addWidth*2);
bcY = bcY - addHeight;
bcHeight = bcHeight + (addHeight*2);

imgHeight, imgWidth = image.shape[:2]

if (bcX<0): bcX = 0
if (bcY<0): bcY = 0
if (bcWidth>imgWidth-1): bcWidth = imgWidth-1
if (bcHeight>imgHeight-1): bcHeight = imgHeight-1


print (bcX, bcY, bcWidth, bcHeight)

roi = image[bcY:bcY+bcHeight, bcX:bcX+bcWidth]

grayRoi = cv2.cvtColor(roi, cv2.COLOR_BGR2GRAY)

cv2.imwrite("C:/inetpub/wwwroot/dd/Images/roi.png", grayRoi)