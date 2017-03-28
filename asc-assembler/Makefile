BUILD_DIR=build
BINARY_NAME=asc_asm

VPATH = src src/core
INCLUDE_DIR = inc

SRCS = $(foreach dir,$(VPATH),$(wildcard $(dir)/*.cpp))
OBJS = $(addprefix $(BUILD_DIR)/,$(notdir $(SRCS:.cpp=.o)))

BUILD = $(CXX) -Wall -O2 $(CXXFLAGS) -I$(INCLUDE_DIR)

all: $(BUILD_DIR) $(BUILD_DIR)/$(BINARY_NAME)

$(BUILD_DIR):
	@mkdir $(BUILD_DIR)

$(BUILD_DIR)/$(BINARY_NAME): $(OBJS)
	$(BUILD) -o $@ $^

$(BUILD_DIR)/%.o: %.cpp
	$(BUILD) -o $@ -c $^

.PHONY: clean
clean:
	@rm -rf $(BUILD_DIR)/
